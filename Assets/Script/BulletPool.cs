using System.Collections.Generic;

public class BulletPool
{
    private static BulletPool instance;
    public static BulletPool Instance
    {
        private set { }
        get
        {
            if (instance == null)
                instance = new BulletPool();
            return instance;
        }
    }

    private List<Bullet> bullets;
    private int currentIdx = 0;

    public int GetBulletCout()
    {
        return bullets.Count;
    }

    public void AddBullet(Bullet bullet)
    {
        if (bullets == null)
            bullets = new List<Bullet>();
        bullets.Add(bullet);
    }

    public void RemoveBullet(Bullet bullet)
    {
        if (bullets != null && bullets.Contains(bullet))
            bullets.Remove(bullet);
    }

    public Bullet GetCurrentIdleBullet()
    {
        if (bullets.Count == 0)
            return null;

        Bullet output = bullets[currentIdx];

        if (output.IsShooting)
            return null;

        currentIdx += 1;

        if (currentIdx >= bullets.Count)
            currentIdx = 0;

        return output;
    }
}
