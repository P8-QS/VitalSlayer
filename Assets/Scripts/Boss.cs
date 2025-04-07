using UnityEngine;

public class Boss : Enemy
{

    public float[] fireballSpeed = { 2.5f, -2.5f };
    public float distance = 0.25f;
    public Transform[] fireballs;

    private void Update()
    {

        for (int i = 0; i < fireballs.Length; i++)
        {
            fireballs[i].position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i]) * distance, Mathf.Sin(Time.time * fireballSpeed[i]) * distance, 0);
        }

    }
    
    protected override void Death()
    {
        Destroy(gameObject);
        int xp = ExperienceManager.Instance.AddBoss(1);
        GameSummaryManager.Instance.AddBoss();
        GameManager.instance.ShowText("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1, 1.0f);
        GameSummaryManager.Instance.Show();
    }


}
