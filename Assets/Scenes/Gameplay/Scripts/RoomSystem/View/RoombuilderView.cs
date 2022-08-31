using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProyectG.Gameplay.RoomSystem.Room;

namespace ProyectG.Gameplay.RoomSystem.View
{
    public class RoombuilderView : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("MAIN REFERENCES")]
        [SerializeField] private GameObject holder = null;
        [SerializeField] private Button exit = null ;
        [SerializeField] private Button build = null ;

        [SerializeField] private EventSystem eventSystem = null;
        [SerializeField] private GraphicRaycaster graphicsRaycaster = null;
        [SerializeField] private RoomView prefabRoomView = null;

        [Header("ROOMS AVIABLE")]
        [SerializeField] private List<RoomModel> allRooms = null;
        #endregion

        #region PRIVATE_FIELDS
        private string idSelectedRoom = string.Empty;
        private Action<bool> onViewToggle = null;
        private Action<Vector3> onSelectedRoom = null;
        private Dictionary<string, RoomView> rooms = new Dictionary<string, RoomView>();
        #endregion

        #region PROPERTIES
        public Action<Vector3> OnSelectedRoom { get => onSelectedRoom; set => onSelectedRoom = value; }
        public Action<bool> OnViewToggle { get => onViewToggle; set => onViewToggle = value; }
        public bool IsActive { get { return holder.gameObject.activeSelf; } }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (!holder.activeInHierarchy)
                return;

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(OnSelectedRoomView())
                {
                    Debug.Log("Selected room " + idSelectedRoom);
                }
                else
                {
                    Debug.Log("Not room selected");
                    idSelectedRoom = string.Empty;

                }
            }

            if (build == null)
                return;

            build.enabled = idSelectedRoom != null;
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            ToggleView(false);

            exit.onClick.AddListener(() => { ToggleView(false); } );

            build.enabled = idSelectedRoom != null;

            for (int i = 0; i < allRooms.Count; i++)
            {
                if (allRooms[i] != null)
                {
                    RoomView roomView = Instantiate(prefabRoomView, Vector3.zero, Quaternion.identity ,holder.transform);
                    roomView.transform.localPosition = allRooms[i].viewPosition;
                    roomView.Init(allRooms[i]);
                    rooms.Add(allRooms[i].id, roomView);
                }
            }
        }

        public void ToggleView(bool state)
        {
            holder.SetActive(state);

            OnViewToggle?.Invoke(IsActive);
        }

        public RoomView GetSelectedRoom()
        {
            return rooms[idSelectedRoom];   
        }
        #endregion

        #region PRIVATE_METHODS
        private bool OnSelectedRoomView()
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            graphicsRaycaster.Raycast(pointerEventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.TryGetComponent(out RoomView selectedRoom))
                {
                    idSelectedRoom = selectedRoom.roomModel.id;

                    onSelectedRoom?.Invoke(GetSelectedRoom().roomModel.worldPosition);
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}