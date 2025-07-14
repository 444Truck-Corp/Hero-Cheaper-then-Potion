using System;
using UnityEngine;

public enum TileMapCharacterType
{
    Hero,
    Diner,
    Quest,
    Shop,
}

[RequireComponent(typeof(SpriteRenderer))]
public class TileMapCharacterCore : Poolable
{
    public TileMapCharacterType CharacterType { get; protected set; }
    protected bool _canAutoFinding;

    [SerializeField] protected Emotion _emotion;
    [SerializeField] protected CharacterMovement _movement;

    public Vector2Int Position => _movement.Position;
    public GuildLocationEventType TargetType { get; protected set; }

    public virtual void Initialize(string textureName)
    {
        _movement.Initialize(textureName, TargetType, _canAutoFinding);
        Clear();
    }

    public void Clear()
    {
        _movement.Clear();
        _emotion.Clear();
    }

    public void SetTargetTilePosition(Vector2Int position)
    {
        _movement.ReleaseLocation();
        _movement.SetTargetTilePosition(position);
    }

    public void SetTargetLocation(EventLocation location)
    {
        _movement.SetLocation(location);
    }

    public void SetMoveCommand(Action onMoveComplete = null)
    {
        _movement.SetMoveCommand(onMoveComplete);
    }

    public virtual void SetOrder()
    {
        _emotion.SetTimer(10.0f, OnFailed);
        _emotion.SetLeftClickEvent(OnClickOrder);
    }

    protected virtual void OnFailed()
    {
        TileMapManager.Instance.OnDinerCharacterExited(this);
    }

    protected virtual void OnClickOrder()
    {
        _emotion.Clear();
    }
}