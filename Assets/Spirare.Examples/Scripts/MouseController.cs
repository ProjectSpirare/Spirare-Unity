﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spirare.Examples
{
    public class MouseController : MonoBehaviour
    {
        [SerializeField]
        private Transform equipmentOrigin;

        private Camera mainCamera;

        private WasmBehaviour equipment = null;

        private bool Equipping => equipment != null;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (equipmentOrigin == null)
            {
                equipmentOrigin = mainCamera.transform;
            }
        }

        void Update()
        {
            //RotateCamera();

            var leftClick = Input.GetMouseButtonDown(0);
            var middleClick = Input.GetMouseButtonDown(2);
            var mousePosition = Input.mousePosition;

            if (middleClick)
            {
                if (Equipping)
                {
                    equipment.InvokeEvent(WasmEventType.Unequip);
                    equipment = null;
                }
                else
                {
                    Equip(mousePosition);
                }
            }

            UpdateEquipmentTransform();

            if (leftClick)
            {
                if (Equipping)
                {
                    equipment.InvokeEvent(WasmEventType.Use);
                }
                else
                {
                    Select(mousePosition);
                }
            }
        }
        
        private void UpdateEquipmentTransform()
        {
            if (!Equipping)
            {
                return;
            }

            var origin = equipmentOrigin;
            equipment.transform.SetPositionAndRotation(origin.position, origin.rotation);
        }

        private void Equip(Vector3 mousePosition)
        {
            if (!TryGetSelectedObject(mousePosition, out var wasm))
            {
                return;
            }
            var pomlElement = wasm.GetComponent<PomlElementComponent>();

            if (pomlElement.Equipable)
            {
                equipment = wasm;
                equipment.InvokeEvent(WasmEventType.Equip);
            }
        }

        private void Select(Vector3 mousePosition)
        {
            if (!TryGetSelectedObject(mousePosition, out var wasm))
            {
                return;
            }
            wasm.InvokeEvent(WasmEventType.Select);
        }

        private bool TryGetSelectedObject(Vector3 screenPos, out WasmBehaviour wasm)
        {
            var ray = mainCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit))
            {
                wasm = null;
                return false;
            }

            var hitObject = hit.collider.gameObject;
            wasm = hitObject.GetComponentInParent<WasmBehaviour>();
            return wasm != null;
        }
    }
}
