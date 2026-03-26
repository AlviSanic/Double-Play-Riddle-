using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common {
    public class StartButtonController : MonoBehaviour {

        [SerializeField]
        private GameObject entranceText;
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private GameObject playCardRoot;
        [SerializeField]
        private GameObject playCardFront;
        [SerializeField]
        private GameObject playCardCover;
        [SerializeField]
        private GameObject[] otherCards;

        public event Action OnStackDeck;
        public event Action OnFlipCard;
        public event Action OnExitDeck;
        public event Action OnDoneExitAnimation;

        private void OnPlayClicked() {
            StartCoroutine(AnimateTheCardsOutRoutine());
        }

        private IEnumerator AnimateTheCardsOutRoutine() {
            playButton.GetComponent<Image>().enabled = false;
            entranceText.SetActive(false);
            var duration = 0.5f;

            foreach (var card in otherCards) {
                card.transform.DOLocalRotate(Vector3.zero, duration * 0.7f, RotateMode.Fast).SetEase(Ease.InBack);
                card.transform.DOLocalMove(Vector3.zero, duration * 0.7f).SetEase(Ease.InBack);
            }
            OnStackDeck?.Invoke();
            yield return new WaitForSeconds(duration * 0.8f);

            playCardRoot.transform.DOScale(1.2f, duration * 0.4f).SetEase(Ease.InSine).OnComplete(() => {
                playCardRoot.transform.DOScale(1.0f, duration * 0.4f).SetEase(Ease.OutSine).OnComplete(() => {
                });
            });

            playCardFront.transform.DOScaleX(0f, duration * 0.4f).SetEase(Ease.InBack).OnComplete(() => {
                playCardFront.SetActive(false);
                playCardCover.SetActive(true);
                playCardCover.transform.localScale = new Vector3(0f, 1f, 1f);
                playCardCover.transform.DOScaleX(1f, duration * 0.4f).SetEase(Ease.OutSine).OnComplete(() => {
                });
            });
            OnFlipCard?.Invoke();
            yield return new WaitForSeconds(duration * 0.8f);

            transform.DOMoveY(transform.position.y - 250f, duration * 0.7f).SetEase(Ease.InBack).OnComplete(() => {
            });
            OnExitDeck?.Invoke();
            yield return new WaitForSeconds(duration * 0.8f);

            OnDoneExitAnimation?.Invoke();
        }

        private void Awake() {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClicked);
        }
    }
}
