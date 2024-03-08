using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ѿ� ������Ʈ Ǯ��

public class BulletPool : MonoBehaviourPunCallbacks
{
    public static BulletPool bulletinstance;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    int poolSize = 5;

    public List<GameObject> bulletPool;

    private void Awake()
    {
        if (bulletinstance == null)
        {
            bulletinstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() // �⺻ �Ѿ� ����
    {
        bulletPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = PhotonNetwork.Instantiate("Bullet", gameObject.transform.position, gameObject.transform.rotation);
            bulletPool.Add(bullet);
            bullet.GetComponent<Bullets>().BulletSleep();
        }
    }

    public GameObject GetBullet() // ��Ȱ��ȭ�Ǿ� �ִ� �Ѿ� ȣ��, ���� �� ����
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        GameObject newBullet = PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation);
        bulletPool.Add(newBullet);
        return newBullet;
    }
}
