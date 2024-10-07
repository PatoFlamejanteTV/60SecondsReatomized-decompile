using System;
using System.Collections;
using Cinemachine;
using FMODUnity;
using Rewired;
using RG.Parsecs.Common;
using RG.Parsecs.EventEditor;
using RG.SecondsRemaster;
using RG.SecondsRemaster.Menu;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
	private const int PLAYER_INDEX = 0;

	private const string RUNNING_ANIM_PARAM = "running";

	private const string GRABBING_ANIM_PARAM = "grabbing";

	private const string DROPPING_ANIM_PARAM = "dropping";

	private const float SMALL_INPUT_THRESHOLD = 0.41f;

	[SerializeField]
	[Header("References")]
	private Rigidbody _rigidbody;

	[SerializeField]
	private GameObject _thirdPersonCameraPrefab;

	[SerializeField]
	private Transform _cameraLookAt;

	[SerializeField]
	[Header("Character Controller")]
	private float _rotationSpeed;

	[SerializeField]
	private float _maxForwardVelocity;

	[SerializeField]
	private float _maxBackwardVelocity;

	[SerializeField]
	private float _maxStrafeVelocity;

	[SerializeField]
	private float _maxForwardVelocityInPreGame;

	[SerializeField]
	private float _maxBackwardVelocityInPreGame;

	[SerializeField]
	private float _maxStrafeVelocityInPreGame;

	[SerializeField]
	private float _verticalForce = 1f;

	[SerializeField]
	private float _horizontalForce = 1f;

	[SerializeField]
	private float _horizontalAcceleration;

	[SerializeField]
	private float _verticalAcceleration;

	[SerializeField]
	private float _directionChangeVelocityChange = 0.5f;

	[SerializeField]
	private float _moveLockTimeout;

	[SerializeField]
	private float _pushForce;

	[Header("Animations and VFX")]
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private ParticleSystem[] _trailParticles;

	[SerializeField]
	private float _particlesActivationMinVelocity;

	[SerializeField]
	private GameObject _collisionEffectPrefab;

	[SerializeField]
	[Range(0f, 100f)]
	private int _collisionEffectChance;

	[SerializeField]
	private float _collisionEffectTimeout;

	[SerializeField]
	private Vector3 _collisionEffectMinOffset;

	[SerializeField]
	private Vector3 _collisionEffectMaxOffset;

	[SerializeField]
	[EventRef]
	private string _scavengeCollisionSoundName;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityMouse;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityGamepad;

	[SerializeField]
	private GlobalFloatVariable _rotationSensitivityKeyboard;

	[SerializeField]
	private GlobalIntVariable _controlModeVariable;

	private EPlayerInput _currentlySetupControlMode;

	[SerializeField]
	private float _gamepadRotationMultiplier = 10f;

	private Player _player;

	private float _rotationHorizontal;

	private float _moveHorizontal;

	private float _moveVertical;

	private Transform _transform;

	private bool _blockMovement;

	private bool _isMovementLimited;

	private bool _useSensitiveRotation = true;

	private float _rotationMultiplier = 1f;

	private float _horizontalVelocity;

	private float _verticalVelocity;

	private float _currentVerticalForce;

	private float _currentHorizontalForce;

	private int _lastHorizontalDirection;

	private int _lastVerticalDirection;

	private float _lastMoveBlockTimestamp;

	private float _moveLockEndTimestamp;

	private float _smoothingMultiplier = 1f;

	private bool _isSmokeTrailActive;

	private static readonly int Grabbing = Animator.StringToHash("grabbing");

	private static readonly int Dropping = Animator.StringToHash("dropping");

	private static readonly int Running = Animator.StringToHash("running");

	private float _maxForwardVelocityPow;

	private float _nextCollisionEffectTime;

	private GlobalFloatVariable _currentRotationSensitivity;

	private float _afterPauseHorizontalInput;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(0);
		_transform = base.transform;
		_maxForwardVelocityPow = Mathf.Pow(_maxForwardVelocity, 2f);
		_rotationMultiplier = 1f;
		EPlayerInput ePlayerInput = (_currentlySetupControlMode = (EPlayerInput)_controlModeVariable.Value);
		_useSensitiveRotation = true;
		switch (ePlayerInput)
		{
		case EPlayerInput.MOUSE_ONLY:
			_currentRotationSensitivity = _rotationSensitivityMouse;
			break;
		case EPlayerInput.KEYBOARD_MOUSE:
			_currentRotationSensitivity = _rotationSensitivityMouse;
			break;
		case EPlayerInput.KEYBOARD:
			_currentRotationSensitivity = _rotationSensitivityKeyboard;
			_useSensitiveRotation = false;
			break;
		case EPlayerInput.GAMEPAD:
			_currentRotationSensitivity = _rotationSensitivityGamepad;
			break;
		case EPlayerInput.TOUCH_ANALOGUE:
		case EPlayerInput.TOUCH_DIGITAL:
			break;
		}
	}

	private void Start()
	{
		SpawnCamera();
	}

	private void Update()
	{
		CheckIfControlModeChanged();
		GetInputs();
		if (_player.controllers.GetLastActiveController() is Joystick)
		{
			CutSmallInputs();
		}
		if (CanMove())
		{
			ControlAnimation();
		}
	}

	private void FixedUpdate()
	{
		RotateCharacter();
		if (CanMove())
		{
			TryToSmoothMove();
			MoveCharacter();
			LimitVelocity();
			ProcessDirectionChange();
			TryToActivateTrailParticles();
		}
		else
		{
			_lastMoveBlockTimestamp = Time.time;
			_moveLockEndTimestamp = _lastMoveBlockTimestamp + _moveLockTimeout;
			TryToResetMove();
		}
	}

	private void OnCollisionStay(Collision other)
	{
		Rigidbody rigidbody = other.rigidbody;
		if (rigidbody == null || rigidbody.isKinematic)
		{
			return;
		}
		Vector3 position = _transform.position;
		Vector3 normalized = (other.contacts[0].point - position).normalized;
		rigidbody.AddForce(normalized * _pushForce, ForceMode.Impulse);
		if (ShouldSpawnCollisionEffect())
		{
			StatsManager.Instance.AddGlobalData("ScavengeCollision", 1);
			_nextCollisionEffectTime = Time.realtimeSinceStartup + _collisionEffectTimeout;
			UnityEngine.Object.Instantiate(position: new Vector3(position.x + UnityEngine.Random.Range(_collisionEffectMinOffset.x, _collisionEffectMaxOffset.x), position.y + UnityEngine.Random.Range(_collisionEffectMinOffset.y, _collisionEffectMaxOffset.y), position.z), original: _collisionEffectPrefab, rotation: Quaternion.identity).transform.SetParent(_transform);
			AudioManager.PlaySoundAtPoint(_scavengeCollisionSoundName, _transform);
			DestructionEffector component = rigidbody.GetComponent<DestructionEffector>();
			if (component != null)
			{
				component.Hit();
			}
		}
	}

	private void CheckIfControlModeChanged()
	{
		EPlayerInput value = (EPlayerInput)_controlModeVariable.Value;
		if (_currentlySetupControlMode != value)
		{
			_useSensitiveRotation = true;
			switch (value)
			{
			case EPlayerInput.MOUSE_ONLY:
				_currentRotationSensitivity = _rotationSensitivityMouse;
				break;
			case EPlayerInput.KEYBOARD_MOUSE:
				_currentRotationSensitivity = _rotationSensitivityMouse;
				break;
			case EPlayerInput.KEYBOARD:
				_currentRotationSensitivity = _rotationSensitivityKeyboard;
				_useSensitiveRotation = false;
				break;
			case EPlayerInput.GAMEPAD:
				_currentRotationSensitivity = _rotationSensitivityGamepad;
				break;
			}
			_currentlySetupControlMode = value;
		}
	}

	private bool ShouldSpawnCollisionEffect()
	{
		if (_collisionEffectPrefab != null && HasInput() && _nextCollisionEffectTime <= Time.realtimeSinceStartup)
		{
			return UnityEngine.Random.Range(0, 100) < _collisionEffectChance;
		}
		return false;
	}

	private void SpawnCamera()
	{
		CinemachineVirtualCamera component = UnityEngine.Object.Instantiate(_thirdPersonCameraPrefab, base.transform).GetComponent<CinemachineVirtualCamera>();
		component.Follow = _cameraLookAt;
		CinemachineBrain.SoloCamera = component;
	}

	private void GetInputs()
	{
		_rotationHorizontal = _player.GetAxis(14);
		_moveHorizontal = _player.GetAxis(2);
		_moveVertical = _player.GetAxis(3);
	}

	private void CutSmallInputs()
	{
		if (Mathf.Abs(_moveHorizontal) < 0.41f)
		{
			_moveHorizontal = 0f;
		}
		if (Mathf.Abs(_moveVertical) < 0.41f)
		{
			_moveVertical = 0f;
		}
	}

	private bool HasInput()
	{
		if (Mathf.Approximately(_moveVertical, 0f))
		{
			return !Mathf.Approximately(_moveHorizontal, 0f);
		}
		return true;
	}

	private void TryToResetMove(bool vertical = true, bool horizontal = true)
	{
		if (vertical || horizontal)
		{
			if (vertical)
			{
				_currentVerticalForce = 0f;
				_moveVertical = 0f;
				_verticalVelocity = 0f;
			}
			if (horizontal)
			{
				_currentHorizontalForce = 0f;
				_moveHorizontal = 0f;
				_horizontalVelocity = 0f;
			}
			if (_rigidbody != null)
			{
				_rigidbody.velocity = new Vector3(horizontal ? 0f : _rigidbody.velocity.x, 0f, vertical ? 0f : _rigidbody.velocity.z);
			}
		}
	}

	private bool TryToSmoothMove()
	{
		if (_moveLockEndTimestamp >= Time.time)
		{
			_smoothingMultiplier = Mathf.InverseLerp(_lastMoveBlockTimestamp, _moveLockEndTimestamp, Time.time);
			return true;
		}
		return false;
	}

	private void TryToLimitMove(float multiplier, bool vertical = true, bool horizontal = true)
	{
		if (vertical || horizontal)
		{
			if (vertical)
			{
				_currentVerticalForce *= multiplier;
				_moveVertical *= multiplier;
				_verticalVelocity *= multiplier;
			}
			if (horizontal)
			{
				_currentHorizontalForce *= multiplier;
				_moveHorizontal *= multiplier;
				_horizontalVelocity *= multiplier;
			}
			if (_rigidbody != null)
			{
				_rigidbody.velocity = new Vector3(horizontal ? (_rigidbody.velocity.x * multiplier) : _rigidbody.velocity.x, 0f, vertical ? (_rigidbody.velocity.z * multiplier) : _rigidbody.velocity.z);
			}
		}
	}

	private void MoveCharacter()
	{
		_currentVerticalForce = Mathf.Lerp(_currentVerticalForce, _verticalForce, _verticalAcceleration * Time.fixedDeltaTime);
		_currentHorizontalForce = Mathf.Lerp(_currentHorizontalForce, _horizontalForce, _horizontalAcceleration * Time.fixedDeltaTime);
		float num = _moveVertical * _currentVerticalForce;
		float num2 = 0f;
		num2 = ((!(Mathf.Abs(_moveVertical) > 0.01f) || _player.controllers.GetLastActiveController() is Joystick) ? (_moveHorizontal * _currentHorizontalForce) : ((_moveHorizontal > 0.01f) ? (_maxStrafeVelocity * Time.fixedDeltaTime) : ((!(_moveHorizontal < -0.01f)) ? 0f : ((0f - _maxStrafeVelocity) * Time.fixedDeltaTime))));
		_rigidbody.AddRelativeForce(new Vector3(num2 * _smoothingMultiplier, 0f, num * _smoothingMultiplier), ForceMode.VelocityChange);
	}

	private int GetDirection(float force)
	{
		if (Mathf.Approximately(0f, force))
		{
			return 0;
		}
		if (force > 0f)
		{
			return 1;
		}
		return -1;
	}

	private void ProcessDirectionChange()
	{
		int direction = GetDirection(_moveHorizontal);
		int direction2 = GetDirection(_moveVertical);
		bool flag = false;
		bool flag2 = false;
		if (direction != _lastHorizontalDirection)
		{
			flag = true;
		}
		if (direction2 != _lastVerticalDirection)
		{
			flag2 = true;
		}
		if (flag || flag2)
		{
			TryToLimitMove(_directionChangeVelocityChange, flag2, flag);
		}
		_lastHorizontalDirection = direction;
		_lastVerticalDirection = direction2;
	}

	private void ControlAnimation()
	{
		bool value = Math.Abs(_moveHorizontal) > 0f || Math.Abs(_moveVertical) > 0f;
		_animator.SetBool(Running, value);
	}

	private void LimitVelocity()
	{
		bool flag = !Mathf.Approximately(_moveVertical, 0f);
		bool flag2 = !Mathf.Approximately(_moveHorizontal, 0f);
		if (!flag)
		{
			_currentVerticalForce = 0f;
		}
		if (!flag2)
		{
			_currentHorizontalForce = 0f;
		}
		Vector3 direction = _transform.InverseTransformDirection(_rigidbody.velocity);
		_verticalVelocity = direction.z;
		_horizontalVelocity = direction.x;
		if (flag2 && direction.sqrMagnitude > _maxForwardVelocityPow)
		{
			if (_verticalVelocity > 0f)
			{
				direction = direction.normalized;
			}
			else if (_verticalVelocity < 0f)
			{
				direction = direction.normalized;
			}
			_verticalVelocity = direction.z;
			_horizontalVelocity = direction.x;
		}
		else
		{
			float max = (flag ? GetCurrentMaxForwardVelocity() : 0f);
			float num = (flag ? GetCurrentMaxBackwardVelocity() : 0f);
			float num2 = (flag2 ? GetCurrentMaxStrafeVelocity() : 0f);
			_verticalVelocity = Mathf.Clamp(_verticalVelocity, 0f - num, max);
			_horizontalVelocity = Mathf.Clamp(_horizontalVelocity, 0f - num2, num2);
			direction = new Vector3(_horizontalVelocity, direction.y, _verticalVelocity);
		}
		_rigidbody.velocity = _transform.TransformDirection(direction);
	}

	private void RotateCharacter()
	{
		if (_blockMovement || _rigidbody == null)
		{
			return;
		}
		if (PauseMenuControl.IS_AFTER_PAUSE)
		{
			if (_rotationHorizontal != 0f)
			{
				_afterPauseHorizontalInput = _rotationHorizontal;
				PauseMenuControl.IS_AFTER_PAUSE = false;
				StartCoroutine(ClearAfterPauseHorizontalInputRoutine());
				return;
			}
		}
		else if (_rotationHorizontal == _afterPauseHorizontalInput)
		{
			return;
		}
		Quaternion rotation = _rigidbody.rotation;
		float num = _rotationSpeed * _currentRotationSensitivity.Value;
		_rotationMultiplier = ((_controlModeVariable.Value == 3) ? _gamepadRotationMultiplier : 1f);
		float num2 = num * _rotationHorizontal * _rotationMultiplier;
		if (_useSensitiveRotation)
		{
			num2 *= Time.fixedDeltaTime;
		}
		_rigidbody.MoveRotation(Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + num2, rotation.eulerAngles.z));
	}

	private IEnumerator ClearAfterPauseHorizontalInputRoutine()
	{
		yield return new WaitForSeconds(0.1f);
		_afterPauseHorizontalInput = 0f;
	}

	private bool CanMove()
	{
		if (!_animator.GetBool(Grabbing) && !_animator.GetBool(Dropping) && _rigidbody != null)
		{
			return !_blockMovement;
		}
		return false;
	}

	private void TryToActivateTrailParticles()
	{
		bool flag = _verticalVelocity > _particlesActivationMinVelocity;
		if (_isSmokeTrailActive && !flag)
		{
			if (Mathf.Approximately(_horizontalVelocity, 0f))
			{
				SetParticlesActive(active: false);
			}
		}
		else if (!_isSmokeTrailActive && flag)
		{
			SetParticlesActive(active: true);
		}
	}

	private void SetParticlesActive(bool active)
	{
		for (int i = 0; i < _trailParticles.Length; i++)
		{
			if (active)
			{
				if (!_trailParticles[i].gameObject.activeSelf)
				{
					_trailParticles[i].gameObject.SetActive(value: true);
				}
				_trailParticles[i].Play();
			}
			else
			{
				_trailParticles[i].Stop();
			}
		}
		_isSmokeTrailActive = active;
	}

	public void SetMovementBlocked(bool block)
	{
		_blockMovement = block;
		StopMovement();
	}

	public void StopMovement()
	{
		_animator.SetBool(Running, value: false);
		_rigidbody.velocity = Vector3.zero;
		SetParticlesActive(active: false);
	}

	public void SetMovementLimited(bool limit)
	{
		_isMovementLimited = limit;
	}

	private float GetCurrentMaxForwardVelocity()
	{
		if (!_isMovementLimited)
		{
			return _maxForwardVelocity;
		}
		return _maxForwardVelocityInPreGame;
	}

	private float GetCurrentMaxBackwardVelocity()
	{
		if (!_isMovementLimited)
		{
			return _maxBackwardVelocity;
		}
		return _maxBackwardVelocityInPreGame;
	}

	private float GetCurrentMaxStrafeVelocity()
	{
		if (!_isMovementLimited)
		{
			return _maxStrafeVelocity;
		}
		return _maxStrafeVelocityInPreGame;
	}
}
