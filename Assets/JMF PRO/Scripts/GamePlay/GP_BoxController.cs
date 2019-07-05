using UnityEngine;
using System.Collections;

public class GP_BoxController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.transform.tag == "animQua")
        {
            coll.transform.GetComponent<CircleCollider2D>().enabled = false;
            coll.transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, Random.Range(3, 8));
            Hashtable hh = new Hashtable();
            hh.Add("rotation", new Vector3(0, 0, Random.Range(0, 360)));
            hh.Add("time", 1);
            iTween.RotateTo(coll.gameObject, hh);
            Destroy(coll.gameObject, 1f);
        }
    }
}
