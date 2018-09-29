using UnityEngine;
using System.Collections.Generic;
using Common.Data;
namespace Gamekit3D.Network
{

    public delegate void RecvHit(IChannel channel, Message message);


    public class NetworkEntity : MonoBehaviour
    {

        public ICreatureBehavior creatureBehavior;

        public NetworkEntity parent;
        public List<NetworkEntity> children = new List<NetworkEntity>();

        public EntityType type;
        public int id;
        public bool canClone = false;
        public void BuildTree()
        {
            parent = transform.parent == null ? null : transform.parent.GetComponent<NetworkEntity>();
            children.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                NetworkEntity child = transform.GetChild(i).GetComponent<NetworkEntity>();
                if (child != null)
                {
                    children.Add(child);
                }
            }
        }

        public DEntity ToDEntity()
        {
            DEntity entity = new DEntity();

            entity.name = name;

            entity.pos.x = transform.position.x;
            entity.pos.y = transform.position.y;
            entity.pos.z = transform.position.z;

            entity.rot.x = transform.rotation.x;
            entity.rot.y = transform.rotation.y;
            entity.rot.z = transform.rotation.z;
            entity.rot.w = transform.rotation.w;

            entity.canClone = canClone;
            Damageable damageable = GetComponent<Damageable>();
            if (damageable != null)
            {
                entity.maxHP = damageable.maxHitPoints;
                entity.HP = damageable.currentHitPoints;
            }
            entity.type = (int)type;
            foreach (NetworkEntity child in children)
            {
                DEntity childEntity = child.ToDEntity();
                childEntity.parent = entity;
                entity.children.Add(childEntity);
            }
            return entity;
        }
        void Awake()
        {
            BuildTree();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
