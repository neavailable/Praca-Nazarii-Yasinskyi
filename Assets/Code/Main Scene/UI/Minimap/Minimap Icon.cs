using UnityEngine;


namespace MainScene.UI.Minimap
{
    // A dot that stays flat above its owner, whatever the owner's rotation or scale.
    // It disappears together with its owner, e.g. a defeated enemy.
    [RequireComponent(typeof(SpriteRenderer))]
    public class MinimapIcon : MonoBehaviour
    {
        [SerializeField] private float _size = 1.5f, _height = 6.0f;

        private float _spriteSize = 1.0f;


        private void Awake()
        {
            int layer = LayerMask.NameToLayer(MinimapView.LayerName);
            if (layer >= 0) gameObject.layer = layer;

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            if (sprite != null) _spriteSize = sprite.bounds.size.x;
        }

        private void LateUpdate()
        {
            Transform owner = transform.parent;
            if (owner == null) return;

            transform.SetPositionAndRotation
            (
                owner.position + Vector3.up * _height,
                Quaternion.Euler(90.0f, 0.0f, 0.0f)
            );

            float ownerScale = owner.lossyScale.x;
            if (ownerScale > 0.0f) transform.localScale = Vector3.one * (_size / _spriteSize / ownerScale);
        }
    }
}
