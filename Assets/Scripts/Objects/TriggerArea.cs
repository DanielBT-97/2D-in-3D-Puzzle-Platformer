using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// This script is used to keep track of all the objects that are to be moved to match a moving platform.
    /// The object containing this script will have a layer that filters out everything but players and other objects allowed to interact with the platforms.
    /// </summary>
	public class TriggerArea : MonoBehaviour {

		private List<Rigidbody> _rigidbodiesInArea = new List<Rigidbody>();
        public List<Rigidbody> RigidbodiesInArea {
            get { return _rigidbodiesInArea; }
        }

        public Rigidbody GetRigidInArea(int index) {
            if(index > _rigidbodiesInArea.Count) {
                if(GlobalSettings.DebugMode) Debug.LogException(new System.Exception("Rigidbody with index ( " + index + " ) is out of range. Current length: " + _rigidbodiesInArea.Count), this.gameObject);
                return _rigidbodiesInArea[0];
            }

            return _rigidbodiesInArea[index];
        }

        public void ClearList() {
            _rigidbodiesInArea.Clear();
        }

		//When a gameobject activates the trigger enter it gets added to the list.
		void OnTriggerEnter(Collider collider) {
			if(collider.attachedRigidbody != null) {
				_rigidbodiesInArea.Add(collider.attachedRigidbody);
			}
		}

		//When a gameobject activates the TriggerExit it gets removed from the list.
		void OnTriggerExit(Collider collider) {
			if(collider.attachedRigidbody != null) {
				_rigidbodiesInArea.Remove(collider.attachedRigidbody);
			}
		}
	}
