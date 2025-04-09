using System;

[Serializable]
public class Node
{
    public int X;
    public int Y;
    public int G; // 이동 비용
    public int H; // 휴리스틱(목적지까지 예상 비용)
    public int F => G + H; // 총 비용
    public bool IsWall;
    public Node Parent;

    public Node(bool isWall, int x, int y)
    {
        IsWall = isWall;
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Node other)
        {
            return X == other.X && Y == other.Y;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
}
