using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hi_Team
{
    public class InputHandler : MonoBehaviour
    {
        PlayerControls inputActions;
        PlayerManager player;

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input;
        public bool a_Input;
        public bool x_Input;
        public bool y_Input;

        public bool tap_rb_Input;
        public bool hold_rb_Input;
        public bool tap_rt_Input;

        public bool lb_Input;
        public bool tap_lb_Input;
        public bool tap_lt_Input;


        public bool jumb_Input;
        public bool inventory_Input;
        public bool lockOnInput;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;


        public bool sprintFlag;
        public bool rollFlag;
        public bool twoHandFlag;
        public bool lockOnFlag;
        public bool fireFlag;
        public bool comboFlag;
        public bool inventoryFlag;
        public float rollInputTimer;

        public Transform criticalAttackRayCastStartPoint;
               

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            
            player = GetComponent<PlayerManager>();
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.RB.performed += i => tap_rb_Input = true;
                inputActions.PlayerActions.HoldRB.performed += i => hold_rb_Input = true;
                inputActions.PlayerActions.HoldRB.canceled += i => hold_rb_Input = false;
                inputActions.PlayerActions.RT.performed += i => tap_rt_Input = true;
                inputActions.PlayerActions.TapLB.performed += i => tap_lb_Input = true;
                inputActions.PlayerActions.LB.performed += i => lb_Input = true;
                inputActions.PlayerActions.LB.canceled += i => lb_Input = false;
                inputActions.PlayerActions.LT.performed += i => tap_lt_Input = true;
                inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
                inputActions.PlayerActions.A.performed += i => a_Input = true;
                inputActions.PlayerActions.X.performed += i => x_Input = true;
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;
                inputActions.PlayerActions.Jumb.performed += i => jumb_Input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
                inputActions.PlayerActions.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerActions.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
                inputActions.PlayerActions.Y.performed += i => y_Input = true;
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            if (player.isDead)
            {
                return;
            }

            HanleMoveInput();
            HandleRollInput();

            HandleHoldRBInput();
            HandleHoldLBInput();

            HandleTapLBInput();
            HandleTapRBInput();
            HandleTapLTInput();
            HandleTapRTInput();

            HandleLockOnInput();
            HandleTwoHandInput();

            HandleAttackInput(); 
            HandleQuickSlotsInput();
            HandleInteractingButtonInput();
            HandleJumbInput();
            HandleInventoryInput();

        }

        private void HanleMoveInput()
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput()
        {
            
            if (b_Input)
            {
                rollInputTimer += Time.deltaTime;
                if (player.playerStatsManager.currentStamina <= 0)
                {
                    b_Input = false;
                    sprintFlag = false;
                }
                if (moveAmount > 0.5f && player.playerStatsManager.currentStamina > 0)
                {
                    sprintFlag = true;
                }
            }
            else
            {
                sprintFlag = false;
                if (rollInputTimer>0 && rollInputTimer < 0.5f)
                {
                    
                    rollFlag = true;
                }
                rollInputTimer = 0;
            }
        }

        private void HandleTapRBInput()
        {
            if (tap_rb_Input)
            {
                tap_rb_Input = false;
                if (player.playerInventoryManager.rightWeapon.tap_RB_Action != null)
                {
                    player.UpdateWhichHandCharacterUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.tap_RB_Action.PerformAction(player);
                }
            }
        }

        private void HandleTapRTInput()
        {
            if (tap_rt_Input)
            {
                tap_rt_Input = false;
                if (player.playerInventoryManager.rightWeapon.tap_RT_Action != null)
                {
                    player.UpdateWhichHandCharacterUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.tap_RT_Action.PerformAction(player);
                }
            }
        }

        private void HandleTapLTInput()
        {
            if (tap_lt_Input)
            {
                tap_lt_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.tap_LT_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.tap_LT_Action.PerformAction(player);
                    }
                }

                else
                {
                    if (player.playerInventoryManager.leftWeapon.tap_LT_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.tap_LT_Action.PerformAction(player);
                    }
                }
            }
        }

        private void HandleHoldLBInput()
        {
            if (player.isInAir || player.isSprinting || player.isFiringSpell)
            {
                lb_Input = false;
                return;
            }
            if (lb_Input)
            {
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.hold_LB_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.hold_LB_Action.PerformAction(player);
                    }
                }
                else
                {
                    if (player.playerInventoryManager.leftWeapon.hold_LB_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.hold_LB_Action.PerformAction(player);
                    }
                }
            }
            else if (lb_Input == false)
            {
                if (player.isAiming)
                {
                    player.isAiming = false;
                    player.uiManager.crossHair.SetActive(false);
                    player.cameraHandler.ResetAimCameraRotations();
                }
                if (player.blockingCollider.blockingCollider.enabled)
                {
                    player.isBlocking = false;
                    player.blockingCollider.DisableBlockingCollider();
                }
            }
        }

        private void HandleTapLBInput()
        {
            if (tap_lb_Input)
            {
                tap_lb_Input = false;
                if (player.isTwoHandingWeapon)
                {
                    if (player.playerInventoryManager.rightWeapon.tap_LB_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.tap_LB_Action.PerformAction(player);
                    }
                }

                else
                {
                    if (player.playerInventoryManager.leftWeapon.tap_LB_Action != null)
                    {
                        player.UpdateWhichHandCharacterUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.tap_LB_Action.PerformAction(player);
                    }
                }
            }
        }

        private void HandleHoldRBInput()
        {
            if (hold_rb_Input)
            {
                if (player.playerInventoryManager.rightWeapon.hold_RB_Action != null)
                {
                    player.UpdateWhichHandCharacterUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.hold_RB_Action.PerformAction(player);
                }
            }
        }

        private void HandleLockOnInput()
        {
            if (lockOnInput && lockOnFlag == false)
            {
                lockOnInput = false;
                player.cameraHandler.HandleLockOn();
                if (player.cameraHandler.nearesLockOnTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.nearesLockOnTarger;
                    lockOnFlag = false;
                }
            }
            else if (lockOnInput && lockOnFlag)
            {
                lockOnFlag = false;
                lockOnInput = false;
                player.cameraHandler.ClearLockOnTargets();

            }

            if (lockOnFlag && right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                player.cameraHandler.HandleLockOn();

                if(player.cameraHandler.leftLockOnTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.leftLockTarget;
                }
            }
            if (lockOnFlag && right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                player.cameraHandler.HandleLockOn();

                if (player.cameraHandler.rightLockOnTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.rightLockTarget;
                }
            }
            if (player.cameraHandler != null)
            {
                player.cameraHandler.SetCameraHeight();
            }
        }

        private void HandleTwoHandInput()
        {
            if (y_Input)
            {
                y_Input = false;
                twoHandFlag = !twoHandFlag;

                if (twoHandFlag)
                {
                    player.isTwoHandingWeapon = true;
                    player.weaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.weaponSlotManager.LoadTwoHandIKTargets(false)
                }
                else
                {
                    player.isTwoHandingWeapon = false;
                    player.weaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.weaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.leftWeapon, true);
                    player.weaponSlotManager.LoadTwoHandIKTargets(false)
                }
            }
        }

        private void HandleUseConsumableInput()
        {
            if (x_Input)
            {
                x_Input = false;
                player.playerInventoryManager.currentConsumable.AttemptToConsumeItem(player.playerAnimatorManager, player.playerWeaponSlotManager, player.playerEffectsManager);
            }
        }

        private void HandleAttackInput()
        {
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;

            if (rb_Input)
            {
                if (player.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else
                {
                    if (player.isInteracting)
                    {
                        return;
                    }
                    if (player.canDoCombo)
                    {
                        return;
                    }
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }
                
            }
            if (rt_Input)
            {
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);

            }
        }

        private void HandleQuickSlotsInput()
        {
            inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
            if (d_Pad_Right)
            {
                playerInventory.ChangeRightWeapon();
            }
            else if (d_Pad_Left)
            {
                playerInventory.ChhangeLeftWeapon();
            }
        }

        private void HandleInteractingButtonInput()
        {
            inputActions.PlayerActions.A.performed += i => a_Input = true;
        }

        private void HandleJumbInput()
        {
            inputActions.PlayerActions.Jumb.performed += i => jumb_Input = true;

        }
        
        private void HandleInventoryInput()
        {
            inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
            if (inventory_Input)
            {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                }
                else
                {
                    uiManager.CloseSelectWindow();
                }
            }
        }
    }
