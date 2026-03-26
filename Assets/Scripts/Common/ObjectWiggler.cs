using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Common {
    public class ObjectWiggler : MonoBehaviour {

        [SerializeField]
        private bool intervalFirst = true;
        [SerializeField]
        [Min(0.01f)]
        private float interval = 3f;
        [SerializeField]
        [Range(0.01f, 1.0f)]
        private float stepDuration = 0.3f;
        [SerializeField]
        [Range(5f, 180f)]
        private float angle = 5f;
        [SerializeField]
        [Min(1)]
        private int repetition = 5;

        private float _cooldown = 0f;
        private bool _isWiggling = false;

        private void Start() {
            Reset();
        }

        private void Update() {
            if (_isWiggling) {
                return;
            }

            _cooldown = Mathf.Max(_cooldown - Time.deltaTime, 0f);
            if (_cooldown == 0f) {
                StartCoroutine(WiggleRoutine());
                _isWiggling = true;
            }
        }

        private void Reset() {
            _cooldown = intervalFirst ? interval : 0f;
            _isWiggling = false;
        }

        private IEnumerator WiggleRoutine() {
            transform.localRotation = Quaternion.identity;

            transform.DOLocalRotate(new Vector3(0f, 0f, angle), stepDuration * 0.5f, RotateMode.Fast).SetEase(Ease.Linear);
            yield return new WaitForSeconds(stepDuration * 0.5f);

            for (int i = 0; i < repetition; i++) {
                transform.DOLocalRotate(new Vector3(0f, 0f, -angle), stepDuration, RotateMode.Fast).SetEase(Ease.Linear);
                yield return new WaitForSeconds(stepDuration);
                transform.DOLocalRotate(new Vector3(0f, 0f, angle), stepDuration, RotateMode.Fast).SetEase(Ease.Linear);
                yield return new WaitForSeconds(stepDuration);
            }

            transform.DOLocalRotate(new Vector3(0f, 0f, 0.0f * angle), stepDuration * 0.5f, RotateMode.Fast).SetEase(Ease.Linear);
            yield return new WaitForSeconds(stepDuration * 0.5f);

            _isWiggling = false;
            _cooldown = interval;
        }
    }
}
