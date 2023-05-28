using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Player controller HSM. Tree viewable in play mode only.
    /// </summary>
    public class CharacterStateMachine : HSM
    {
        [SerializeField] private Transform playerBody;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private LayerMask ground;

        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float dashMult = 1.5f;
        [SerializeField] private float dashTime = 0.3f;
        [SerializeField] private float jumpForce = 10;

        private float facing = 1;
        private Vector2 moveDir;
        private float speedMult = 1;
        private bool isGrounded = false;
        private bool dashing = false;


        private void Awake()
        {
            //Grounded states
            var grounded = new BaseState(this, "Grounded");
            grounded.AddDecorator(new DashDecorator(this));
            {
                //Mover
                var groundMover = new CharacterMoveableState(this, "Mover");
                groundMover.AddDecorator(new JumpDecorator(this));
                {
                    //Idle
                    var idle = new BaseState(this, "Idle");
                    idle.AddDecorator(new DelayDecorator(this, 2f));

                    var wait = new BaseState(this, "Wait");
                    wait.AddDecorator(new AnimatorDecorator(this, "Wait"));
                    var cheer = new BaseState(this, "Cheer");
                    cheer.AddDecorator(new AnimatorDecorator(this, "Cheer"));

                    idle.AddChild(wait);
                    idle.AddChild(cheer);
                    idle.AddTransition(StateEvent.delay, wait, cheer);

                    //Running
                    var running = new BaseState(this, "Running");
                    running.AddDecorator(new AnimatorDecorator(this, "Running"));

                    groundMover.AddChild(idle);
                    groundMover.AddChild(running);
                    groundMover.AddTransition(StateEvent.run, idle, running);
                    groundMover.AddTransition(StateEvent.idle, running, idle);
                }

                //Ground dash
                var groundDash = new CharacterDashState(this, "Dash", dashTime);
                groundDash.AddDecorator(new JumpDecorator(this));

                //Grounded adds
                grounded.AddChild(groundMover);
                grounded.AddChild(groundDash);
                grounded.AddTransition(StateEvent.dash, groundMover, groundDash);
                grounded.AddTransition(StateEvent.delay, groundDash, groundMover);
            }

            //In air states
            var inAir = new BaseState(this, "In Air");
            inAir.AddDecorator(new DashDecorator(this));
            {
                //Mover
                var airMover = new CharacterMoveableState(this, "Mover");
                airMover.AddDecorator(new AnimatorDecorator(this, "Jumping"));

                var singleJump = new BaseState(this, "Jump 1");
                singleJump.AddDecorator(new JumpDecorator(this));
                var doubleJump = new BaseState(this, "Jump 2");

                airMover.AddChild(singleJump);
                airMover.AddChild(doubleJump);
                airMover.AddTransition(StateEvent.jump, singleJump, doubleJump);

                //Air dash
                var airDash = new CharacterDashState(this, "Dash", dashTime);

                //In air adds
                inAir.AddChild(airMover);
                inAir.AddChild(airDash);
                inAir.AddTransition(StateEvent.dash, airMover, airDash);
                inAir.AddRevert(StateEvent.delay, airDash);
            }

            //Root
            root = new BaseState(this, "Root");
            root.AddChild(grounded);
            root.AddChild(inAir);
            root.AddTransition(StateEvent.grounded, inAir, grounded);
            root.AddTransition(StateEvent.inAir, grounded, inAir);
            root.EnterState(false);
        }


        private void FixedUpdate()
        {
            //Set velocity
            Vector2 newVel = rb.velocity;
            newVel.x = moveDir.x * moveSpeed * speedMult;
            if(dashing)
            {
                newVel.y = 0;
            }
            else if(moveDir.y == 1)
            {
                newVel.y = 0;
                newVel.y = jumpForce;
                moveDir.y = 0;
            }
            rb.velocity = newVel;

            //Ground check
            float rayLength = 0.1f;
            bool wasGrounded = isGrounded;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, rayLength, ground);
            isGrounded = hit.collider != null;
            if(!wasGrounded && isGrounded)
            {
                InvokeStateEvent(StateEvent.grounded);
                moveDir.x = 0;
            }
            else if(wasGrounded && !isGrounded)
            {
                InvokeStateEvent(StateEvent.inAir);
            }
        }

        #region //Movement
        public void SetXDir(int xDir)
        {
            if(moveDir.x == 0 && xDir != 0)
            {
                InvokeStateEvent(StateEvent.run);
            }
            else if(moveDir.x != 0 && xDir == 0)
            {
                InvokeStateEvent(StateEvent.idle);
            }

            moveDir.x = xDir;
            if(xDir != 0)
            {
                facing = Mathf.Sign(xDir);
                playerBody.localScale = new Vector3(facing, 1, 1);
            }
        }

        public void Jump()
        {
            moveDir.y = 1;
        }

        public void Dash()
        {
            dashing = true;
            moveDir.x = facing;
            speedMult = dashMult;
        }

        public void EndDash()
        {
            dashing = false;
            moveDir.x = 0;
            speedMult = 1;
        }
        #endregion
    }
}
