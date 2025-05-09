using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header ("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO> ();
    QuestionSO currentQuestion;

    [Header ("Answers")]
    [SerializeField] GameObject [] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly;
    
    [Header ("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header ("Timer")]

    [SerializeField] Image timerImage;
    Timer timer;
    
    
    [Header ("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    
    void Start()
    {
        timer= FindObjectOfType<Timer>();
        DisplayQuestion();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
    }
    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if(timer.loadNextQuestion)
        {
            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false; 
        }

        else if (!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }


    }

    void DisplayAnswer (int index)
    {
        Image buttonImage;

        if (index == currentQuestion.GetCorrectAnswer())
        {
            questionText.text = "Correct!";
            buttonImage = answerButtons[index].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }

        else
        
        {
            correctAnswerIndex = currentQuestion.GetCorrectAnswer();
            string correctAnswer = currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text = "Sorry the correct answer was:\n" + correctAnswer;
            buttonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = correctAnswerSprite;
        }

    
    }
    public void OnAnswerSelected (int index)
    {
        
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState (false);
        timer.CancelTimer();
        scoreText.text = "Score:" + scoreKeeper.CalculateScore() + "%";
    }


    void GetNextQuestion()
    {
        
        if (questions.Count>0)
        {
        SetButtonState(true);
        SetDefaultButtonSprites();
        RandomQuestion();
        DisplayQuestion();
        scoreKeeper.IncrementQuestionsSeen();
        }
        
        
        
    }

    void RandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions [index];
        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
    }
    void DisplayQuestion()
    {
        questionText.text= currentQuestion.GetQuestion();

    //
        for (int i=0; i<answerButtons.Length; i++)
        {
        TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text= currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState (bool state)
    {
        for(int i = 0; i<answerButtons.Length; i++)
        {
            Button button = answerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        for (int i = 0; i<answerButtons.Length; i++)
        {
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }
}