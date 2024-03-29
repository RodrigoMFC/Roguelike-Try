using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Health UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;
    [SerializeField] private GameObject gameOverRestartButton;
    [Header("Message UI")]
    [SerializeField] private int sameMessageCount = 0; //Read-only
    [SerializeField] private string lastMessage; //Read-only
    [SerializeField] private bool isMessageHistoryOpen = false; //Read-only
    [SerializeField] private GameObject messageHistory;
    [SerializeField] private GameObject messageHistoryContent;
    [SerializeField] private GameObject lastFiveMessagesContent;
    [SerializeField] private TextMeshProUGUI messagePrefab;

    public bool IsMessageHistoryOpen { get => isMessageHistoryOpen; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reloads scene
    }

    public void SetHealthMax(int maxHp)
    {
        hpSlider.maxValue = maxHp;
    }

    public void SetHealth(int hp, int maxHp)
    {
        hpSlider.value = hp;
        hpSliderText.text = $"HP: {hp}/{maxHp}";
        gameOverRestartButton.SetActive(hp <= 0);
    }

    public void ToggleMessageHistory()
    {
        messageHistory.SetActive(!messageHistory.activeSelf);
        isMessageHistoryOpen = messageHistory.activeSelf;
    }

    public void AddMessage(string newMessage, string colorHex)
    {
        lastMessage = newMessage;

        TextMeshProUGUI message = Instantiate(messagePrefab) as TextMeshProUGUI;
        message.text = newMessage;
        message.color = Color.white;
        message.transform.SetParent(messageHistoryContent.transform, false);

        for (int i = 0; i < lastFiveMessagesContent.transform.childCount; i++)
        {
            if (messageHistoryContent.transform.childCount - 1 < i)
            {
                return;
            }

            TextMeshProUGUI lastFiveHistoryChild = lastFiveMessagesContent.transform.GetChild(lastFiveMessagesContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI messageHistoryChild = messageHistoryContent.transform.GetChild(messageHistoryContent.transform.childCount - 1 - i).GetComponent<TextMeshProUGUI>();
            lastFiveHistoryChild.text = messageHistoryChild.text;
            lastFiveHistoryChild.color = messageHistoryChild.color;
        }
    }

    private Color GetColorFromHex(string v)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(v, out color))
        {
            return color;
        }
        else
        {
            Debug.Log("GetColorFromHex: Could not parse color from string");
            return Color.white;
        }
    }
}