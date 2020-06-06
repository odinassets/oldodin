using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldOdin
{
    /// <summary>
    /// This script is just an example of how to use Animazing 
    /// and it is not intended to be a complete solution for a third-person controller
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class SimpleThirdPersonController : MonoBehaviour
    {
        [Header("Basic Movement")]
        public AnimationClip idle;
        public AnimationClip walkForward;
        public AnimationClip walkBack;
        public AnimationClip walkLeft;
        public AnimationClip walkRight;  
        public AnimationClip leftTurn;
        public AnimationClip rightTurn;

        [Header("Jumps")]
        public AnimationClip jump;
        public AnimationClip jumpForward;
        public AnimationClip jumpBack;
        public AnimationClip jumpRight;
        public AnimationClip jumpLeft;

        [Header("Rolling")]
        public AnimationClip roll;
        public AnimationClip rollBack;
        public AnimationClip rollLeft;
        public AnimationClip rollRight;
        public AnimationClip soccerTackle;

        [Header("Actions")]
        public AnimationClip punch;
        public AnimationClip kick;
        public AnimationClip aim;

        [Header("Rections")]
        public AnimationClip hurt;
        public AnimationClip die;

        [Header("Params")]
        public float angularSpeed = 120;
        public float hp = 100;

        Animazing animazing;
        CharacterController controller;

        private void Awake()
        {
            animazing = GetComponent<Animazing>();
            //sets Idle as the default animation for the base layer (0)
            animazing.SetLayerDefaultAnimation(0, idle);
            //define "Empty" as the default state for layer 1
            animazing.SetLayerDefaultState(1, "Empty"); //I am using two layers in this example. Layer 1 has the aiming animation
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Get user awsd input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            //Basic Movement
            if (v > 0)
                animazing.Play(walkForward, 1, 0.1f);
            if (v < 0)
                animazing.Play(walkBack, 1);
            if (h > 0)
                animazing.Play(walkRight, 1);
            if (h < 0)
                animazing.Play(walkLeft, 1);

            //Jumps
            if (Input.GetButtonDown("Jump"))
            {
                if (v > 0.1f)
                    animazing.Play(jumpForward, 3, 0.1f);
                else if (v < -0.1f)
                    animazing.Play(jumpBack, 3, 0.1f);
                else if (h > 0.1f)
                    animazing.Play(jumpRight, 3, 0.5f);
                else if (h < -0.1f)
                    animazing.Play(jumpLeft, 3, 0.5f);
                else animazing.Play(jump, 3);
            }

            //Actions
            if (Input.GetButtonDown("Fire1"))
                animazing.Play(punch, 2, 0.1f);
            if (Input.GetMouseButtonDown(2))
                animazing.Play(kick, 2);
            if (Input.GetMouseButton(1))
                animazing.PlayLayer(1, aim, 4);

            //Rolling
            if (v > 0.1f && Input.GetAxis("Mouse ScrollWheel") < -0.01f)
                animazing.Play(soccerTackle, 3, 0.05f);
            if (v < - 0.1f && Input.GetAxis("Mouse ScrollWheel") < -0.01f)
                animazing.Play(rollBack, 3, 0.1f);
            if (h < -0.1f && Input.GetAxis("Mouse ScrollWheel") > 0.01f)
                animazing.Play(rollLeft, 3, 0.5f);
            if (h > 0.1f && Input.GetAxis("Mouse ScrollWheel") > 0.01f)
                animazing.Play(rollRight, 3, 0.5f);
            if (Input.GetAxis("Mouse ScrollWheel") > 0.01f)
                animazing.Play(roll, 3);

            //Rotation
            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX != 0 && animazing.CanPlay(10))
                transform.Rotate(Vector3.up, mouseX * angularSpeed * Time.deltaTime);
            if (mouseX > 0)
                animazing.Play(rightTurn, 0.5f);
            if (mouseX < 0)
                animazing.Play(leftTurn, 0.5f);

            HideShowCursor();
            
            if (!animazing.IsPlaying(3)) //if the player is not jumping
                ApplyGravity();
        }

        void HideShowCursor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        void ApplyGravity()
        {
            controller.Move(Vector3.down * 5 * Time.deltaTime);
        }

        public void Hurt(float damage = 40)
        {
            if (animazing.CanPlay(4))
            {
                hp -= damage;
                if (hp > 0)
                    animazing.Play(hurt, 4);
                else{
                    animazing.Play(die, 10, 0.05f, Mathf.Infinity);
                    StartCoroutine(Restart());
                }
            }
        }

        IEnumerator Restart()
        {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}