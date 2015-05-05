using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

namespace CardMatchNewGUI
{
    public class Card : MonoBehaviour
    {
        public GameManager gameManager;
        public int no, type;
        public Image bg, fg, on;
        Transform tr;
        bool ok = true;
        public Sprite[] sprites;

        void Awake()
        {
            tr = transform;
            sprites = new Sprite[40];
            for (int i = 0; i < 40; i++)
                sprites[i] = Resources.Load<Sprite>("Icons/rpg" + (i + 1).ToString("00"));
        }

        public void OnPress(bool isPressed)
        {
            if (!ok) return;
            if (gameManager)
                gameManager.SetChoice(this);
        }

        public void SetCard(int n, int t)
        {
            ok = true;
            no = n;
            type = t;
            if (t == -1) fg.enabled = false;
            else fg.sprite = sprites[t];
        }

        public void FlipOffCard()
        {
            ok = false;
            tr.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
            SequenceParms sparams = new SequenceParms();
            Sequence mySequence = new Sequence(sparams);
            TweenParms parms;
            parms = new TweenParms().Prop("localRotation", Quaternion.AngleAxis(90f, Vector3.up)).Ease(EaseType.Linear).OnComplete(HideFg);
            mySequence.Append(HOTween.To(tr, 0.15f, parms));
            parms = new TweenParms().Prop("localRotation", Quaternion.AngleAxis(0f, Vector3.up)).Ease(EaseType.Linear);
            mySequence.Append(HOTween.To(tr, 0.15f, parms));
            mySequence.Play();
        }

        public void FlipOnCard()
        {
            ok = true;
            tr.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
            SequenceParms sparams = new SequenceParms();
            Sequence mySequence = new Sequence(sparams);
            TweenParms parms;
            parms = new TweenParms().Prop("localRotation", Quaternion.AngleAxis(90f, Vector3.up)).Ease(EaseType.Linear).OnComplete(ShowFg);
            mySequence.Append(HOTween.To(tr, 0.15f, parms));
            parms = new TweenParms().Prop("localRotation", Quaternion.AngleAxis(0f, Vector3.up)).Ease(EaseType.Linear);
            mySequence.Append(HOTween.To(tr, 0.15f, parms));
            mySequence.Play();
        }

        void ShowFg()
        {
            fg.enabled = true;
        }

        void HideFg()
        {
            fg.enabled = false;
        }

        void Update()
        {

        }
    }
}