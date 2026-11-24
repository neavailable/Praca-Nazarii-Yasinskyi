using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using AllScenes.Statistics;
using Configuration.Data;


namespace MainScene.UI.Timer
{
    // Explains the goal and the rules before the level starts; the game waits until Start is pressed.
    // The level's elements are listed with their icons below the rules.
    public class LevelIntro : MonoBehaviour
    {
        private const string _rules =
            "Goal: collect all coins - then the finish opens - and reach it before the time runs out.\n" +
            "Shoot: drag back from the ball and release. Push nudges a stuck ball.";

        [SerializeField] private TMP_Text _title, _text, _levelText;
        [SerializeField] private Button _startButton;

        private LevelRulesData _levelRules;


        [Inject]
        private void Constructor(LevelRulesData levelRules) => _levelRules = levelRules;

        private void Start()
        {
            string levelRules = _levelRules.Intro + "\n" + "Time limit: " + TimeFormatter.Format(_levelRules.TimeLimit);

            _title.text = _levelRules.Title;
            if (_levelText != null)
            {
                _text.text      = _rules;
                _levelText.text = levelRules;
            }
            else
            {
                _text.text = _rules + "\n\n" + levelRules;
            }

            _startButton.onClick.AddListener(Close);
            Time.timeScale = 0.0f;
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(Close);
            Time.timeScale = 1.0f;
        }


        private void Close()
        {
            Time.timeScale = 1.0f;
            gameObject.SetActive(false);
        }
    }
}
