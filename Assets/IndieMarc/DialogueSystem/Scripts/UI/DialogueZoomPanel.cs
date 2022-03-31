using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.DialogueSystem
{
    public class DialogueZoomPanel : UIPanel
    {

        public UIPanel dialogue_bg;
        public RectTransform portrait_group1;
        public RectTransform portrait_group2;
        public Image portrait1;
        public Image portrait2;
        public GameObject bubble1;
        public GameObject bubble2;
        public Text title;
        public Text text;

        private string current_text = "";
        private Coroutine text_anim;
        private Vector2 portrait1_start;
        private Vector2 portrait2_start;
        private bool text_anim_completed = true;
        private CinematicActorPortrait zoomed_in_portrait1 = null;
        private CinematicActorPortrait zoomed_in_portrait2 = null;

        private static DialogueZoomPanel _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;

            portrait1_start = portrait_group1.anchoredPosition;
            portrait2_start = portrait_group2.anchoredPosition;
            portrait1.sprite = null;
            portrait2.sprite = null;
        }

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void Update()
        {
            base.Update();

            Vector2 diff1 = portrait1_start - portrait_group1.anchoredPosition;
            float move1 = Mathf.Min(420f * Time.deltaTime, diff1.magnitude);
            portrait_group1.anchoredPosition += diff1.normalized * move1;

            Vector2 diff2 = portrait2_start - portrait_group2.anchoredPosition;
            float move2 = Mathf.Min(420f * Time.deltaTime, diff2.magnitude);
            portrait_group2.anchoredPosition += diff2.normalized * move2;

            if (zoomed_in_portrait1)
                zoomed_in_portrait1.transform.position = portrait1.transform.position + new Vector3(zoomed_in_portrait1.offset.x, zoomed_in_portrait1.offset.y);
            if (zoomed_in_portrait2)
                zoomed_in_portrait2.transform.position = portrait2.transform.position + new Vector3(zoomed_in_portrait2.offset.x, zoomed_in_portrait2.offset.y);

        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            if (dialogue_bg != null)
                dialogue_bg.Show();
        }

        public override void Hide(bool instant = false)
        {
            base.Hide(instant);

        }

        public override void AfterHide()
        {
            base.AfterHide();
            if (dialogue_bg != null)
                dialogue_bg.Hide();
            portrait1.sprite = null;
            portrait2.sprite = null;
            HidePortrait1();
            HidePortrait2();
        }

        private void HidePortrait1()
        {
            if (zoomed_in_portrait1)
            {
                zoomed_in_portrait1.PlayDefault();
                zoomed_in_portrait1.Hide();
            }
        }

        private void HidePortrait2()
        {
            if (zoomed_in_portrait2)
            {
                zoomed_in_portrait2.PlayDefault();
                zoomed_in_portrait2.Hide();
            }
        }

        public void SetDialogue(string title, Sprite img, CinematicActorPortrait portrait, string text, int side, string anim, bool flipped = false)
        {
            this.title.text = title;
            this.text.text = "";
            current_text = text;
            bubble1.SetActive(false);
            bubble2.SetActive(false);
            gameObject.SetActive(true); //Allow starting coroutine
            
            if (side >= 0)
            {
                if (img != portrait2.sprite || portrait != zoomed_in_portrait2)
                    portrait_group2.anchoredPosition += Vector2.right * 200f;
                if (portrait != zoomed_in_portrait2)
                    HidePortrait2();
                if (zoomed_in_portrait1)
                    zoomed_in_portrait1.PlayDefault();
                zoomed_in_portrait2 = portrait;
                portrait2.sprite = img;
                portrait2.transform.localScale = flipped ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
                portrait2.color = Color.white;
                portrait1.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                bubble2.SetActive(current_text != "");
            }
            else
            {
                if (img != portrait1.sprite || portrait != zoomed_in_portrait1)
                    portrait_group1.anchoredPosition += Vector2.left * 200f;
                if (portrait != zoomed_in_portrait1)
                    HidePortrait1();
                if (zoomed_in_portrait2)
                    zoomed_in_portrait2.PlayDefault();
                zoomed_in_portrait1 = portrait;
                portrait1.sprite = img;
                portrait1.transform.localScale = flipped ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
                portrait1.color = Color.white;
                portrait2.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                bubble1.SetActive(current_text != "");
            }

            if (zoomed_in_portrait1)
            {
                zoomed_in_portrait1.Show();
                zoomed_in_portrait1.PlayAnim(anim);
            }
            if (zoomed_in_portrait2)
            {
                zoomed_in_portrait2.Show();
                zoomed_in_portrait2.PlayAnim(anim);
            }

            portrait1.enabled = portrait1.sprite != null;
            portrait2.enabled = portrait2.sprite != null;
            text_anim_completed = false;
            text_anim = StartCoroutine(AnimateText());
        }

        public void SkipTextAnim()
        {
            this.text.text = current_text;
            text_anim_completed = true;
            StopCoroutine(text_anim);
        }

        public bool IsTextAnimCompleted()
        {
            return text_anim_completed;
        }

        IEnumerator AnimateText()
        {
            for (int i = 0; i < (current_text.Length + 1); i++)
            {
                this.text.text = current_text.Substring(0, i);
                yield return new WaitForSeconds(.03f);
            }
            text_anim_completed = true;
        }

        public static DialogueZoomPanel Get()
        {
            return _instance;
        }
    }

}