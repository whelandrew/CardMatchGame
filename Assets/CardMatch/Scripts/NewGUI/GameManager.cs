using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

namespace CardMatchNewGUI
{

    public class GameManager : MonoBehaviour
    {
        public GameObject cardPrefab;
        int tileSizeX = 2;
        int tileSizeY = 2;
        public int tileWidth = 310;
        public int tileHeight = 310;
        int level = 1;
        int tileTotal, counter;
        float width;
        bool isInput = true;
        bool isGameOver = true;
        Text scoreLabel;
        public AudioSource soundPick, soundYes, soundNo, soundSuccess, soundFailure, soundScore, soundReady;

        int cardTypeTotal = 40;
        Card[] cards;
        Card choiceCard = null;
        GameObject grid;

        int score;

        int totalLevel = 7;
        int[,] tileTable = new int[7, 2] {
        {3,3}, {4,3}, {4,4}, {5,4}, {5,5}, {6,5}, {6,6}
    };
        public int[] pairTable = new int[7]{
        4, 6, 8, 10, 12, 15, 18
    };

        void Start()
        {
            InitGame();
            ReadyGame();
        }

        void InitGame()
        {
            GameObject panelGame = GameObject.Find("Canvas").transform.FindChild("Panel").gameObject;
            grid = panelGame.transform.FindChild("Grid").gameObject;
            scoreLabel = panelGame.transform.FindChild("ScoreLabel").GetComponent<Text>();
        }

        public void ReadyGame()
        {
            isGameOver = false;
            level = 1;
            SetScore(0, false);
            tileSizeX = tileTable[level - 1, 0];
            tileSizeY = tileTable[level - 1, 1];
            tileTotal = tileSizeX * tileSizeY;
            cards = new Card[tileTotal];
            InitGrid();
            ReadyGrid();
            ClearCards();
            StartCoroutine(DelayAction(1.5f, () =>
            {
                soundReady.Play();
                ShowCards();
            }));
        }


        void ReadyGrid()
        {
            int tot = pairTable[level - 1], t;
            counter = tot;

            List<int> tlist = new List<int>(), rlist = new List<int>();
            for (int i = 0; i < cardTypeTotal; i++) tlist.Add(i);
            tlist.Shuffle();
            for (int j = 0; j < 2; j++)
                for (int i = 0; i < tot; i++) rlist.Add(tlist[i]);
            t = tileTotal - tot * 2;
            for (int i = 0; i < t; i++) rlist.Add(tlist[tot * 2 + i]);
            rlist.Shuffle();
            for (int i = 0; i < tileTotal; i++)
            {
                Card cd = cards[i];
                //cd.fg.enabled = true;
                cd.SetCard(i, rlist[i]);
            }
        }

        void InitGrid()
        {
            Transform gridTr = grid.transform;
            foreach (Transform child in gridTr) {
                Destroy(child.gameObject);
            }
            float scale = 2f / tileSizeX;
            float posx = (tileSizeX - 1f) / 2f;
            float posy = (tileSizeY - 1f) / 2f;
            width = tileWidth * scale;
            int n = 0;

            for (int i = 0; i < tileSizeX; i++)
            {
                for (int j = 0; j < tileSizeY; j++)
                {
                    GameObject go = Instantiate(cardPrefab) as GameObject;
                    go.transform.SetParent(grid.transform);
                    go.transform.localScale = new Vector3(scale, scale, 1f);
                    go.transform.localPosition = new Vector3((i - posx) * width, (j - posy) * width);
                    Card card = go.GetComponent<Card>();
                    cards[n] = card;
                    card.gameManager = this;
                    n++;
                }
            }
        }

        void ClearCards()
        {
            choiceCard = null;
            foreach (Card cd in cards)
                if (cd.fg.enabled) cd.fg.enabled = false;
        }

        void HideCards()
        {
            choiceCard = null;
            foreach (Card cd in cards)
                if (cd.fg.enabled) cd.FlipOffCard();
        }

        void ShowCards()
        {
            choiceCard = null;
            foreach (Card cd in cards)
                cd.FlipOnCard();
        }

        public void SetChoice(Card cd)
        {
            if (!isInput) return;
            if (!cd.fg.enabled) return;
            if (counter < 1) return;
            soundPick.Play();
            if (!choiceCard)
            {
                choiceCard = cd;
                cd.on.enabled = true;
                return;
            }
            if (choiceCard == cd)
            {
                cd.on.enabled = false;
                choiceCard = null;
                return;
            }
            isInput = false;
            cd.on.enabled = true;
            if (choiceCard.type == cd.type)
            {
                soundYes.Play();
                Success(cd);
            }
            else
            {
                soundNo.Play();
                Failure(cd);
            }
        }

        void GameOver(bool ok)
        {
            if (isGameOver) return;
            StopAllCoroutines();
            isGameOver = true;
        }

        void LevelUp()
        {
            StartCoroutine(DelayAction(0.5f, () =>
            {
                soundSuccess.Play();
            }));
            StartCoroutine(DelayAction(0.9f, () =>
            {
                HideCards();
            }));

            if (level >= totalLevel)
            {
                level = 0;
                /*
                StartCoroutine(DelayAction(1.5f, () =>
                {
                    GameOver(true);
                }));
                return;
                 */
            }

            StartCoroutine(DelayAction(2.5f, () =>
            {
                level++;
                tileSizeX = tileTable[level - 1, 0];
                tileSizeY = tileTable[level - 1, 1];
                tileTotal = tileSizeX * tileSizeY;
                cards = new Card[tileTotal];
                InitGrid();
                ReadyGrid();
                soundReady.Play();
                ShowCards();
            }));
        }

        public int sampleScore = 0;

        void UpdateTweenScore()
        {
            scoreLabel.text = sampleScore.ToString("#,##0");
        }

        void SetScore(int val, bool isTween)
        {
            sampleScore = score;
            score = val;
            if (isTween)
            {

                TweenParms parms = new TweenParms().Prop("sampleScore", val).Ease(EaseType.Linear).OnUpdate(UpdateTweenScore);
                HOTween.To(this, 1f, parms);
            }
            else scoreLabel.text = val.ToString("#,##0");
        }

        void AddScore(int val)
        {
            SetScore(score + val, true);
        }

        void CalcScore()
        {
            int val = level * level * 100;
            AddScore(val);
        }

        void Success(Card cd)
        {
            if (isGameOver) return;
            CalcScore();
            counter--;
            if (counter < 1)
            {
                LevelUp();
            }

            StartCoroutine(DelayAction(0.1f, () =>
            {
                choiceCard.on.enabled = false;
                cd.on.enabled = false;
                choiceCard.FlipOffCard();
                cd.FlipOffCard();
                choiceCard = null;
                isInput = true;
            }));
        }

        void Failure(Card cd)
        {
            if (isGameOver) return;
            StartCoroutine(DelayAction(0.1f, () =>
            {
                choiceCard.on.enabled = false;
                cd.on.enabled = false;
                choiceCard = null;
                isInput = true;
            }));
        }

        public void GoInfo()
        {
            Application.OpenURL("http://hompy.info");
        }

        public void GoHome()
        {
            Application.OpenURL("http://buntgames.com");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        IEnumerator DelayAction(float dTime, System.Action callback)
        {
            yield return new WaitForSeconds(dTime);
            callback();
        }
    }
}