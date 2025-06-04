using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch;
		public bool fire;
		public bool aim;
		public bool reload;
		public bool peekLeft;
		public bool peekRight;
		public bool nightVision;
		public bool interact;
		public bool heal;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private bool wasAiming = false;

		private PlayerHealth _health;
		private FirstPersonController _controller;
		private CharacterController _characterController;

		public bool playerLocked = false;
		private bool lockCamera = false;

        private void Awake()
        {
            _health = GetComponent<PlayerHealth>();
			_controller = GetComponent<FirstPersonController>();
			_characterController = GetComponent<CharacterController>();
        }

		public void lockPlayer(bool lockCamera)
		{
			playerLocked = true;
			_controller.playerLocked = !lockCamera;
			if (lockCamera) _controller.enabled = false;

			_characterController.enabled = false;

			move = Vector2.zero;

			this.lockCamera = lockCamera;

			if (lockCamera) look = Vector2.zero;

			jump = false;
			sprint = false;
			fire = false;
			aim = false;
			reload = false;
			peekLeft = false;
			peekRight = false;
			nightVision = false;
			heal = false;
        }

		public void unlockPlayer()
		{
            _characterController.enabled = true;
            _controller.enabled = true;
			_controller.playerLocked = false;

			playerLocked = false;
			lockCamera = false;
		}

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			if (playerLocked) return;
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook && !lockCamera)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
            if (playerLocked) return;
            JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
            if (playerLocked) return;
            SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
            if (playerLocked) return;
            CrouchInput(value.isPressed);
		}

		public void OnFire(InputValue value)
		{
            if (playerLocked) return;
            FireInput(value.isPressed);
		}

        public void OnAim(InputValue value)
        {
            if (playerLocked) return;
            AimInput(value.isPressed);
        }

		public void OnReload(InputValue value)
		{
            if (playerLocked) return;
            ReloadInput(value.isPressed);
		}

		public void OnPeekRight(InputValue value)
		{
            if (playerLocked) return;
            PeekRightInput(value.isPressed);
		}

		public void OnPeekLeft(InputValue value)
		{
            if (playerLocked) return;
            PeekLeftInput(value.isPressed);
		}

		public void OnNightVision(InputValue value)
		{
            if (playerLocked) return;
            NightVisionInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
            InteractInput(value.isPressed);
		}

		public void OnHeal(InputValue value)
		{
            if (playerLocked) return;
            HealInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			if (_health.healing) return;
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
            if (newSprintState) crouch = false;
			if (newSprintState)
			{
				if (aim)
				{
					aim = false;
					wasAiming = true;
				}
			}
			else
			{
				if (wasAiming)
				{
					aim = true; 
					wasAiming = false;
				}
			}
        }

		public void CrouchInput(bool newCrouchState)
		{
			if (sprint) return;
			crouch = (newCrouchState) ? !crouch : crouch;
		}

		public void FireInput(bool newFireState)
		{
			fire = newFireState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void PeekRightInput(bool newPeekState)
		{
			if (reload) return;
			peekRight = (newPeekState) ? !peekRight : peekRight;
			peekLeft = (newPeekState) ? false : peekLeft;
		}

		public void PeekLeftInput(bool newPeekState)
		{
            if (reload) return;
            peekLeft = (newPeekState) ? !peekLeft : peekLeft;
            peekRight = (newPeekState) ? false : peekRight;
        }

        public void NightVisionInput(bool newNightVisionState)
        {
			if (_health.healing) return;
            nightVision = (newNightVisionState) ? !nightVision : nightVision;
        }

        public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void HealInput(bool newHealState)
		{
			heal = newHealState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}