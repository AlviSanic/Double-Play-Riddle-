using System;
using System.Collections;
using UnityEngine;
using Utility;
using DG.Tweening;
using TMPro;

namespace Common {
    public class CardController : MonoBehaviour {

        private const float RotationDuration = 0.3f;

        [SerializeField]
        private GameObject front;
        [SerializeField]
        private GameObject backing;
        [SerializeField]
        private SpriteRenderer letter;
        [SerializeField]
        private TextMeshPro hintText;

        public event Action<CardController> OnRotateStart;
        public event Action<CardController> OnRotated;

        public char Letter { private set; get; } = '#';
        public bool IsRotating { private set; get; } = false;
        public CardState State { private set; get; } = CardState.Closed;
        public bool IsHeldByCursor { private set; get; } = false;

        private Tweener _hintFader = null;
        private float _originalPosZ = 0f;

        public void SetLetter(char c, bool showHint = false) {
            transform.localRotation = Quaternion.identity;
            letter.sprite = CardLetterSprites.Instance.GetSprite(c);
            var letterOffset = CardLetterOffsets.GetOffset(c);
            letter.transform.localPosition = letterOffset.position;
            letter.transform.localRotation = Quaternion.Euler(0f, 0f, letterOffset.rotation);
            Letter = c.ToString().ToUpper()[0];

            if (showHint) {
                if (_hintFader != null) {
                    _hintFader.Kill();
                }
                hintText.text = Letter.ToString().ToLower();
                hintText.gameObject.SetActive(true);
                hintText.color = new Color(hintText.color.r, hintText.color.g, hintText.color.b, 1f);
                _hintFader = hintText.DOColor(new Color(hintText.color.r, hintText.color.g, hintText.color.b, 0f), 1f).SetEase(Ease.Linear).OnComplete(() => {
                    hintText.gameObject.SetActive(false);
                    _hintFader = null;
                });
            }
        }

        public void Rotate() {
            if (IsRotating || State != CardState.Open) {
                return;
            }
            StartCoroutine(RotateRoutine());
        }

        public void FlipOpen(bool snap = true) {
            if (snap) {
                front.gameObject.SetActive(true);
                backing.gameObject.SetActive(false);
                letter.gameObject.SetActive(true);
                State = CardState.Open;
                return;
            }
            State = CardState.FlippingOpen;
            StartCoroutine(FlipOpenRoutine());
        }

        public void FlipClose(bool snap = true) {
            if (snap) {
                front.gameObject.SetActive(false);
                backing.gameObject.SetActive(true);
                letter.gameObject.SetActive(false);
                State = CardState.Closed;
                return;
            }
            State = CardState.FlippingClose;
            StartCoroutine(FlipCloseRoutine());
        }

        public void HoldByCursor() {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, _originalPosZ - 1f);
            transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            IsHeldByCursor = true;
        }

        public void ReleaseByCursor() {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, _originalPosZ);
            transform.localScale = Vector3.one;
            IsHeldByCursor = false;
        }

        private IEnumerator FlipOpenRoutine() {
            front.gameObject.SetActive(false);
            backing.gameObject.SetActive(true);
            letter.gameObject.SetActive(false);
            var proceed = false;
            transform.DOScale(1.2f, 0.15f).SetEase(Ease.OutSine).OnComplete(() => {
                transform.DOScale(1f, 0.15f).SetEase(Ease.InSine).OnComplete(() => {
                });
            });
            transform.DOScaleX(0f, 0.15f).SetEase(Ease.InSine).OnComplete(() => {
                front.gameObject.SetActive(true);
                backing.gameObject.SetActive(false);
                letter.gameObject.SetActive(true);
                transform.DOScaleX(1f, 0.15f).SetEase(Ease.OutSine).OnComplete(() => {
                    proceed = true;
                });
            });

            yield return new WaitUntil(() => proceed);
            State = CardState.Open;
        }

        private IEnumerator FlipCloseRoutine() {
            front.gameObject.SetActive(true);
            backing.gameObject.SetActive(false);
            letter.gameObject.SetActive(true);
            var proceed = false;
            transform.DOScaleX(0f, 1f).SetEase(Ease.InSine).OnComplete(() => {
                front.gameObject.SetActive(false);
                backing.gameObject.SetActive(true);
                letter.gameObject.SetActive(false);
                transform.DOScaleX(1f, 1f).SetEase(Ease.OutSine).OnComplete(() => {
                    proceed = true;
                });
            });

            yield return new WaitUntil(() => proceed);
            State = CardState.Open;
        }

        private IEnumerator RotateRoutine() {
            IsRotating = true;
            OnRotateStart?.Invoke(this);
            hintText.gameObject.SetActive(false);
            var newLetter = CardPairs.GetPair(Letter);
            var proceed = true;

            proceed = false;
            var letterOffset = CardLetterOffsets.GetOffset(newLetter);
            letter.transform.DOLocalMove(-letterOffset.position, RotationDuration).SetEase(Ease.OutQuart);

            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, _originalPosZ - 1f);
            transform.DOScale(1.2f, RotationDuration * 0.5f).SetEase(Ease.OutSine).OnComplete(() => {
                transform.DOScale(1f, RotationDuration * 0.5f).SetEase(Ease.InSine).OnComplete(() => {
                    transform.localPosition = new Vector3(pos.x, pos.y, _originalPosZ);
                });
            });
            transform.DOLocalRotate(new Vector3(0f, 0f, -180f), RotationDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuart).OnComplete(() => {
                proceed = true;
            });
            yield return new WaitUntil(() => proceed);

            SetLetter(newLetter, true);
            IsRotating = false;
            OnRotated?.Invoke(this);
        }

        private void Start() {
            _originalPosZ = transform.localPosition.z;
        }
    }
}
