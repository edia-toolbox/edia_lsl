using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 
 * Moves a GameObject with a random velocity and angular velocity which are reset every <ResetInterval> seconds.
 * 
 * From the LSL4Unity package (https://github.com/labstreaminglayer/LSL4Unity) 
 */

namespace LSL4Unity.Samples.Complex
{
    public class RandomTransform : MonoBehaviour
    {
        public float ResetInterval = 2.0f;
        public Rigidbody RigidBody;
        private float _elapsedTime = 0.0f;
        private Vector3 _startPosition;

        // Start is called before the first frame update
        void Start()
        {
            RigidBody = gameObject.AddComponent<Rigidbody>();
            RigidBody.useGravity = false;
            // RigidBody.isKinematic = true;
            Vector3 p = gameObject.transform.position;
            _startPosition = new Vector3(p.x, p.y, p.z);
        }

        // Update is called once per frame
        void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= ResetInterval)
            {
                gameObject.transform.position = _startPosition;
                RigidBody.velocity = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
                RigidBody.angularVelocity = new Vector3(Random.Range(-6.0f, 6.0f), Random.Range(-6.0f, 6.0f), Random.Range(-6.0f, 6.0f));
                _elapsedTime = 0.0f;
            }
        }
    }
}