using System;
using UnityEngine;
using Zenject;
using MainScene.UserInput;


namespace MainScene.LineRender
{
    public class LineRenderEnabler : MonoBehaviour
    {
        public Action LineRenderEnabled;
        
        private BallInput _ballInput;
        private LineRenderDrawer _lineRenderDrawer;
        
        
        [Inject]
        private void Constructor(BallInput ballInput, LineRenderDrawer lineRenderDrawer)
        {
            _ballInput        = ballInput;
            _lineRenderDrawer = lineRenderDrawer;
        }

        private void Start()
        {
            _ballInput.BallInputLogic.LeftMouseButtonPressedAction  
                += EnableLineRenderController;
            _ballInput.BallInputLogic.LeftMouseButtonReleasedAction 
                += DisableLineRenderController;
        }

        private void EnableLineRenderController()
        {
            _lineRenderDrawer.gameObject.SetActive(true);
            LineRenderEnabled?.Invoke();
        }

        private void DisableLineRenderController()
        {
            _lineRenderDrawer.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _ballInput.BallInputLogic.LeftMouseButtonPressedAction  
                -= EnableLineRenderController;
            _ballInput.BallInputLogic.LeftMouseButtonReleasedAction 
                -= DisableLineRenderController;
        }
    }
}