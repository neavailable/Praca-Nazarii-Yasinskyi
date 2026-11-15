using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MainScene.UserInput
{
    public class BallInputLogic : IDisposable
    {
        public Action LeftMouseButtonPressedAction, LeftMouseButtonReleasedAction;
        
        private readonly PlayerInput _playerInput;
        private readonly ClickedOnBallChecker _clickedOnBallChecker;
        private bool _clickedOnBall;
        
    
        public BallInputLogic()
        {
            _clickedOnBall = false;

            _playerInput          = new PlayerInput();
            _clickedOnBallChecker = new ClickedOnBallChecker();    
            
            _playerInput.Player.Move.performed += OnPointerPressed;
            _playerInput.Player.Move.canceled  += OnPointerReleased;
            _playerInput.Enable();
        }
        
        public void Dispose()
        {
            _playerInput.Player.Move.performed -= OnPointerPressed;
            _playerInput.Player.Move.canceled  -= OnPointerReleased;
            
            _playerInput.Disable();
            _playerInput.Dispose();
        }

        private void OnPointerPressed(InputAction.CallbackContext _) => LeftMouseButtonPressed();

        private void OnPointerReleased(InputAction.CallbackContext _) => LeftMouseButtonReleased();
        
        
        private void LeftMouseButtonPressed()
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(PlayerPointer.Position);
            if (!_clickedOnBallChecker.CheckClickedOnBall(mouseRay))
            {
                _clickedOnBall = false;
                return;
            }

            _clickedOnBall = true;
            LeftMouseButtonPressedAction?.Invoke();
        }

        private void LeftMouseButtonReleased()
        {
            if (!_clickedOnBall) return;
                
            LeftMouseButtonReleasedAction?.Invoke();
            _clickedOnBall = false;
        }
    }
}
