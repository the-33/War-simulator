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

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private bool wasAiming = false;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}

		public void OnFire(InputValue value)
		{
			FireInput(value.isPressed);
		}

        public void OnAim(InputValue value)
        {
			AimInput(value.isPressed);
        }

		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}

		public void OnPeekRight(InputValue value)
		{
			PeekRightInput(value.isPressed);
		}

		public void OnPeekLeft(InputValue value)
		{
			PeekLeftInput(value.isPressed);
		}

		public void OnNightVision(InputValue value)
		{
			NightVisionInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
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
            nightVision = (newNightVisionState) ? !nightVision : nightVision;
        }

        public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
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