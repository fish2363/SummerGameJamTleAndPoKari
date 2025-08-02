using DG.Tweening;
using Member.ISC.Code.Players;
using UnityEngine;

public class BugPlayerTeleporter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().GetCompo<PlayerHealth>().Ignore = true;
            collision.gameObject.transform.position = Vector2.zero;
            sr.DOFade(0f, 0.3f).SetLoops(3,LoopType.Yoyo).OnComplete(()=>
            {
                sr.DOFade(1f, 0.1f);
                collision.gameObject.GetComponent<Player>().GetCompo<PlayerHealth>().Ignore = false;
            });
        }
    }
}
