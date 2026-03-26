using System;
using UnityEngine;

namespace Common {
    public class CardMover : MonoBehaviour {

        private const float DragThreshold = 20f;

        public event Action<CardController, CardHolderController> OnCardHold;
        public event Action<CardController, CardHolderController> OnCardDrag;

        public Func<CardController, CardHolderController, bool> onValidateCardHold = null;
        public Func<CardHolderController, int, bool, bool> onShiftCards;

        private bool _isDragging = false;
        private CardController _cardOnHold = null;
        private CardHolderController _cardHolderSource = null;
        private Vector3 _mouseDownPos = Vector3.one * -1f;
        private Vector2 _dragCardOffset = Vector2.zero;

        private void Update() {
            if (!enabled) {
                return;
            }

            var doesMouseDownOrUp = false;
            var mousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0)) {
                doesMouseDownOrUp = true;
                var ray = Camera.main.ScreenPointToRay(mousePos);
                var hits = Physics.RaycastAll(ray, 50f);
                foreach (var hit in hits) {
                    if (hit.collider.TryGetComponent<CardHolderController>(out var ch)) {
                        if (ch.Card == null) {
                            continue;
                        }
                        if (!ch.Card.IsRotating && ch.Card.State == CardState.Open) {
                            _cardHolderSource = ch;
                            _cardOnHold = _cardHolderSource.Card;
                            _mouseDownPos = mousePos;
                        }
                        break;
                    }
                }
                _isDragging = false;
            }
            if (Input.GetMouseButtonUp(0)) {
                doesMouseDownOrUp = true;
                if (_cardOnHold != null) {
                    if (_isDragging) {
                        var isHandled = false;
                        var ray = Camera.main.ScreenPointToRay(mousePos);
                        var hits = Physics.RaycastAll(ray, 50f);
                        foreach (var hit in hits) {
                            if (hit.collider.TryGetComponent<CardHolderController>(out var ch)) {
                                if (!ch.IsSnapping && (onValidateCardHold == null || onValidateCardHold.Invoke(_cardOnHold, ch))) {
                                    _cardHolderSource.Clear();
                                    if (ch.Card == null || (onShiftCards != null && onShiftCards.Invoke(ch, 0, false))) {
                                        OnCardHold?.Invoke(_cardOnHold, ch);
                                        ch.Hold(_cardOnHold, false);
                                    }
                                    else {
                                        _cardHolderSource.Hold(_cardOnHold, false);
                                    }
                                }
                                else {
                                    _cardHolderSource.Hold(_cardOnHold, false);
                                }
                                isHandled = true;
                                break;
                            }
                        }
                        if (!isHandled) {
                            _cardHolderSource.Hold(_cardOnHold, false);
                            isHandled = true;
                        }
                    }
                    else {
                        _cardOnHold.Rotate();
                    }
                    _isDragging = false;
                    _cardOnHold = null;
                    _cardHolderSource = null;
                    _mouseDownPos = Vector3.one * -1f;
                    _dragCardOffset = Vector2.zero;
                }
            }

            // When there's no mouse down or up, this frame is for possible dragging
            if (!doesMouseDownOrUp && _cardOnHold != null) {
                if (!_isDragging) {
                    var mousePosDelta = mousePos - _mouseDownPos;
                    if (mousePosDelta.sqrMagnitude >= DragThreshold * DragThreshold) {
                        var mouseDownPosOnWorld = Camera.main.ScreenToWorldPoint(_mouseDownPos);
                        _dragCardOffset.x = _cardOnHold.transform.position.x - mouseDownPosOnWorld.x;
                        _dragCardOffset.y = _cardOnHold.transform.position.y - mouseDownPosOnWorld.y;
                        _isDragging = true;
                        OnCardDrag?.Invoke(_cardOnHold, _cardHolderSource);
                    }
                }

                if (_isDragging) {
                    _cardOnHold.HoldByCursor();
                    var mousePosOnWorld = Camera.main.ScreenToWorldPoint(mousePos);
                    var pos = new Vector3(mousePosOnWorld.x + _dragCardOffset.x, mousePosOnWorld.y + _dragCardOffset.y, _cardOnHold.transform.position.z);
                    _cardOnHold.transform.position = pos;

                    var ray = Camera.main.ScreenPointToRay(mousePos);
                    var hits = Physics.RaycastAll(ray, 50f);
                    foreach (var hit in hits) {
                        if (hit.collider.TryGetComponent<CardHolderController>(out var ch)) {
                            if (onValidateCardHold == null || onValidateCardHold.Invoke(_cardOnHold, ch)) {
                                if (ch.Card != null) {
                                    _cardHolderSource.Clear();
                                    if (onShiftCards != null && onShiftCards.Invoke(ch, ch.transform.position.x - pos.x < 0 ? -1 : 1, true)) {
                                        _cardHolderSource = ch;
                                    }
                                    _cardHolderSource.HoldSilently(_cardOnHold);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
