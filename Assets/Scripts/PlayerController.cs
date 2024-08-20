using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _walkSpeed, _jumpChargeSpeed, _jumpChargeMin, _jumpChargeMax = 3, _xJumpAmount, _groundedChekRadius = 0.1f, _yCheckOffset = 0.02f, _groundCheckLockTime = 0.1f, _playerFriction = 1, _playerAirBounciness = 0, _stunFallDistance, _stunTime;
    [SerializeField]
    private Sprite _standingSprite, _walkingSprite, _jumpChargeLowSprite, _jumpChargeMidSprite, _jumpChargeHighSprite, _jumpingSprite, _fallingSprite, _fallStunSprite;
    private bool _isGrounded, _flipped, _falling;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private LayerMask _playerMask;
    private float _jumpCharge, _lockTimer, _prevY, _stunTimer;
    private PhysicsMaterial2D _playerMaterial;

    private void Start()
    {

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerMask = ~(1 << LayerMask.NameToLayer("Player"));
        _playerMaterial = new PhysicsMaterial2D()
        {

            friction = _playerFriction,
            bounciness = 0

        };

        _rb.sharedMaterial = _playerMaterial;
        GetComponent<CapsuleCollider2D>().sharedMaterial = _playerMaterial;

    }

    private void Update()
    {

        if(_isGrounded)
        {

            _playerMaterial.bounciness = 0;
            _playerMaterial.friction = _playerFriction;

        }
        else
        {

            _playerMaterial.bounciness = _playerAirBounciness;
            _playerMaterial.friction = 0;

        }

        if(_lockTimer > 0)
        {

            _lockTimer -= Time.deltaTime;
            _isGrounded = false;
            return;

        }

        if(_stunTimer > 0)
        {

            _stunTimer -= Time.deltaTime;
            return;

        }

        if(Physics2D.OverlapCircleAll((Vector2)transform.position + (Vector2.up * -_yCheckOffset), _groundedChekRadius, _playerMask).Length > 0)
        {

            _isGrounded = true;

        }
        else
        {

            _isGrounded = false;

        }

        if(_isGrounded && _falling && transform.position.y < _prevY - _stunFallDistance)
        {

            _stunTimer = (_prevY - transform.position.y) / _stunFallDistance * _stunTime;
            _falling = false;
            _spriteRenderer.sprite = _fallStunSprite;
            _prevY = 0;
            return;

        }

        if(_isGrounded)
        {

            _prevY = 0;

            if(Input.GetKey(KeyCode.LeftShift))
            {

                _jumpCharge = 0;

            }
            else if(Input.GetKey(KeyCode.Space))
            {

                _jumpCharge = Mathf.Clamp(_jumpCharge + (_jumpChargeSpeed * Time.deltaTime), _jumpChargeMin, _jumpChargeMax);

                _rb.velocity = new Vector2(0, 0);

                if(_jumpCharge < _jumpChargeMax / 2)
                {

                    _spriteRenderer.sprite = _jumpChargeLowSprite;

                }
                else if(_jumpCharge >= _jumpChargeMax)
                {

                    _spriteRenderer.sprite = _jumpChargeHighSprite;

                }
                else
                {

                    _spriteRenderer.sprite = _jumpChargeMidSprite;

                }
                
            }
            else if(Input.GetKeyUp(KeyCode.Space))
            {

                _rb.velocity = new Vector2(_xJumpAmount * (_flipped ? -1 : 1), _jumpCharge);
                _jumpCharge = 0;
                _lockTimer = _groundCheckLockTime;
                _isGrounded = false;

                _spriteRenderer.sprite = _jumpingSprite;

            }

        }

        if(_isGrounded)
        {

            float xDir = Input.GetAxis("Horizontal");

            if(xDir != 0 && _jumpCharge == 0)
                _rb.velocity = new Vector2(xDir * _walkSpeed, _rb.velocity.y);

            if(xDir != 0)
            {

                if(_jumpCharge == 0)
                    _spriteRenderer.sprite = _walkingSprite;
                _flipped = xDir < 0;
                _spriteRenderer.flipX = _flipped;

            }
            else if(_jumpCharge == 0)
            {

                _spriteRenderer.sprite = _standingSprite;

            }

        }
        else
        {

            if(transform.position.y > _prevY)
                _prevY = transform.position.y;

            if(_rb.velocity.y > 0)
            {

                _spriteRenderer.sprite = _jumpingSprite;

            }
            else
            {

                _spriteRenderer.sprite = _fallingSprite;
                _falling = true;

            }

        }

    }

}
