using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button[] buttons;
    public Button resetButton;
    public Button resetStatisticsButton; // Reference to the Reset Statistics button
    public Button settingsButton;
    public Button homeButton;
    public TextMeshProUGUI winMessageText; // Reference to the UI Text element for win messages
    public TextMeshProUGUI statisticsText; // Reference to the statistics text element
    private string currentPlayer = "X";
    private bool isGameActive = true; // Flag to check if the game is active

    private int playerXWins;
    private int playerOWins;
    private int draws;

    void Start()
    {
        // Find all game buttons with the tag "GameButton" and assign them to the array
        GameObject[] gameButtonObjects = GameObject.FindGameObjectsWithTag("GameButton");
        buttons = new Button[gameButtonObjects.Length];
        for (int i = 0; i < gameButtonObjects.Length; i++)
        {
            buttons[i] = gameButtonObjects[i].GetComponent<Button>();
        }

        InitializeGame();

        // Add click event listener for the reset button
        resetButton.onClick.AddListener(ResetGame);
        // Add click event listener for the reset statistics button
        resetStatisticsButton.onClick.AddListener(ResetStatistics);

        // Load statistics
        LoadStatistics();
        UpdateStatisticsText();
    }

    void InitializeGame()
    {
        isGameActive = true; // Set the game as active at the start

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is not assigned.");
                continue;
            }

            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>(); // Use TextMeshProUGUI
            if (buttonText == null)
            {
                Debug.LogError($"TextMeshProUGUI component missing in button at index {i}.");
                continue;
            }

            buttonText.text = "";
            int index = i; // Local copy to avoid closure issue
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int index)
    {
        if (!isGameActive)
        {
            Debug.Log("Game is not active. No more moves are allowed.");
            return;
        }

        Debug.Log($"Button {index} clicked");

        TextMeshProUGUI buttonText = buttons[index].GetComponentInChildren<TextMeshProUGUI>(); // Use TextMeshProUGUI
        if (buttonText == null)
        {
            Debug.LogError($"TextMeshProUGUI component missing in button at index {index}.");
            return;
        }

        if (buttonText.text == "")
        {
            buttonText.text = currentPlayer;
            Debug.Log($"Setting button {index} text to {currentPlayer}");

            if (CheckWin())
            {
                Debug.Log($"Player {currentPlayer} wins!");
                DisplayWinMessage($"Player {currentPlayer} wins!");
                isGameActive = false; // Set the game as inactive
                if (currentPlayer == "X")
                    playerXWins++;
                else
                    playerOWins++;
                SaveStatistics();
                UpdateStatisticsText();
            }
            else if (CheckDraw())
            {
                Debug.Log("It's a draw!");
                DisplayWinMessage("It's a draw!");
                isGameActive = false; // Set the game as inactive
                draws++;
                SaveStatistics();
                UpdateStatisticsText();
            }
            else
            {
                currentPlayer = currentPlayer == "X" ? "O" : "X";
                Debug.Log($"Current player is now {currentPlayer}");
            }
        }
    }

    bool CheckWin()
    {
        string[,] board = new string[3, 3];
        for (int i = 0; i < buttons.Length; i++)
        {
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            int row = i / 3;
            int col = i % 3;
            board[row, col] = buttonText.text;
        }

        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] != "" && board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2])
            {
                return true;
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] != "" && board[0, col] == board[1, col] && board[1, col] == board[2, col])
            {
                return true;
            }
        }

        // Check diagonals
        if (board[0, 0] != "" && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
        {
            return true;
        }

        if (board[0, 2] != "" && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
        {
            return true;
        }

        return false;
    }

    bool CheckDraw()
    {
        // Check if any button is still empty
        foreach (var button in buttons)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == "")
                return false;
        }

        // If all buttons are filled and no win condition is met, it's a draw
        return true;
    }

    void ResetGame()
    {
        currentPlayer = "X";
        isGameActive = true; // Set the game as active when reset
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogError($"Button at index {i} is not assigned.");
                continue;
            }

            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>(); // Use TextMeshProUGUI
            if (buttonText == null)
            {
                Debug.LogError($"TextMeshProUGUI component missing in button at index {i}.");
                continue;
            }

            buttonText.text = "";
        }

        // Clear the win message text
        winMessageText.text = "";
    }

    void ResetStatistics()
    {
        playerXWins = 0;
        playerOWins = 0;
        draws = 0;
        SaveStatistics();
        UpdateStatisticsText();
    }

    void SaveStatistics()
    {
        PlayerPrefs.SetInt("PlayerXWins", playerXWins);
        PlayerPrefs.SetInt("PlayerOWins", playerOWins);
        PlayerPrefs.SetInt("Draws", draws);
        PlayerPrefs.Save();
    }

    void LoadStatistics()
    {
        playerXWins = PlayerPrefs.GetInt("PlayerXWins", 0);
        playerOWins = PlayerPrefs.GetInt("PlayerOWins", 0);
        draws = PlayerPrefs.GetInt("Draws", 0);
    }

    void UpdateStatisticsText()
    {
        statisticsText.text = $"Player X Wins: {playerXWins}\nPlayer O Wins: {playerOWins}\nDraws: {draws}";
    }

    void DisplayWinMessage(string message)
    {
        winMessageText.text = message;
    }
}
