using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class DartScript : MonoBehaviour
{

    public AudioSource DartHit;
    public GameObject CustomRightHand;
    public Vector3 lastPosition;
    public bool grabbed = false;
    public bool first = true;
    public float d_radius = 0;
    public int points = 0;
    public int total_points = 0;
    public int total_throws = 0;
    Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        lastPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            d_radius = 0;
            points = 0;
            total_points = 0;
            total_throws = 0;
            String score_text = "Score: " + total_points.ToString();
            String dist_text = " Distance: " + 0;
            String point_text = " Points: " + 0;
            String throw_text = " Throws: " + 0;

            

            TextMeshPro Scoreboard = (GameObject.FindWithTag("Scoreboard")).GetComponent<TextMeshPro>();
            Scoreboard.text = score_text + dist_text + point_text + throw_text;
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= 0.5)
        {
            m_Rigidbody.isKinematic = false;
            Vector3 displacement = CustomRightHand.transform.position - lastPosition;

            // set position of dart
            this.transform.position = CustomRightHand.transform.position;
            this.GetComponent<Rigidbody>().velocity = (displacement / Time.deltaTime)*1.4f;

            lastPosition = CustomRightHand.transform.position;
            //Debug.Log(this.transform.eulerAngles);

            if (first) {
                total_throws += 1;
                first = false;
            }
            String score_text = "Score: " + total_points.ToString();
            String dist_text = " Distance: " + d_radius;
            String point_text = " Points: " + points;
            String throw_text = " Throws: " + total_throws.ToString();

            Debug.Log(score_text);
            TextMeshPro Scoreboard = (GameObject.FindWithTag("Scoreboard")).GetComponent<TextMeshPro>();
            Scoreboard.text = score_text + dist_text + point_text + throw_text;
        }
        else {
            grabbed = false;
            first = true;
        }
    }

    void LateUpdate()
    {
        
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision other)
    {

        // hits table
        if (other.gameObject.tag == "DartBoard")
        {
            // play dart hit
            DartHit.Play();

            // Have object stop
            //m_Rigidbody.velocity = Vector3.zero;
            //m_Rigidbody.angularVelocity = Vector3.zero;

            // turn off gravity
            //m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;


            // Get Measurements
            Vector2 origin = new Vector2(other.transform.position.x, other.transform.position.y);
            Vector2 dartHit = new Vector2(this.transform.position.x, this.transform.position.y);
            float x = this.transform.position.x - other.transform.position.x;
            float y = this.transform.position.y - other.transform.position.y;

            float m_angle = Vector2.SignedAngle(origin, dartHit);

            // get angle from origin
            float angle = (float)((Mathf.Atan2(x, y)/ Math.PI) * 180f);
            if (angle < 0) angle += 360f;

            float distance = Vector2.Distance(origin, dartHit);

            

            // Need to get score from hit
            // 20 panels
            // 0 degrees start from top of dartboard

            // Check to see what ring it is in
            float radius = 0.9586873f/2.0f;
            d_radius = distance / radius;
            
            Debug.Log("d_radius: " + d_radius);
            
            // calculate score from region hit by dart
            points = 0;
            bool skip = false;
            bool single_p = false;
            bool double_p = false;
            bool triple_p = false;

            // bullseye red
            if (d_radius >= 0 && d_radius < 0.03286) {
                points = 50;
                skip = true;
            }
            // bullseye green
            if (d_radius >= 0.03286 && d_radius < .0935) {
                points = 25;
                skip = true;
            }
            // black/white
            if (d_radius >= .0935 && d_radius < .477185) {
                single_p = true;
            }
            // inner red/green
            if (d_radius >= .477185 && d_radius < .5331) {
                triple_p = true;
            }
            // black/white
            if (d_radius >= .5331 && d_radius < .750) {
                single_p = true;
            }
            // outer red/green
            if (d_radius >= .750 && d_radius <= .809) {
                double_p = true;
            }
            // out
            if (d_radius > .809) {
                points = 0;
                skip = true;
            }

            if (!skip) {
                // Section 20
                if (angle >= 351 || angle < 9) {
                    if (single_p)
                    {
                        points = 20;
                    }
                    else if (double_p)
                    {
                        points = 40;
                    }
                    else if (triple_p) {
                        points = 60;
                    }
                }
                // Section 1
                else if (angle >= 9 && angle < 27) {
                    if (single_p)
                    {
                        points = 1;
                    }
                    else if (double_p)
                    {
                        points = 2;
                    }
                    else if (triple_p)
                    {
                        points = 3;
                    }
                }

                // Section 18
                else if (angle >= 27 && angle < 45) {
                    if (single_p)
                    {
                        points = 18;
                    }
                    else if (double_p)
                    {
                        points = 36;
                    }
                    else if (triple_p)
                    {
                        points = 54;
                    }
                }

                // Section 4
                else if (angle >= 45 && angle < 63) {
                    if (single_p)
                    {
                        points = 4;
                    }
                    else if (double_p)
                    {
                        points = 8;
                    }
                    else if (triple_p)
                    {
                        points = 12;
                    }
                }

                // Section 13
                else if (angle >= 63 && angle < 81) {
                    if (single_p)
                    {
                        points = 13;
                    }
                    else if (double_p)
                    {
                        points = 26;
                    }
                    else if (triple_p)
                    {
                        points = 39;
                    }
                }

                // Section 6
                else if (angle >= 81 && angle < 99) {
                    if (single_p)
                    {
                        points = 6;
                    }
                    else if (double_p)
                    {
                        points = 12;
                    }
                    else if (triple_p)
                    {
                        points = 18;
                    }
                }

                // Section 10
                else if (angle >= 99 && angle < 117) {
                    if (single_p)
                    {
                        points = 10;
                    }
                    else if (double_p)
                    {
                        points = 20;
                    }
                    else if (triple_p)
                    {
                        points = 30;
                    }
                }

                // Section 15
                else if (angle >= 117 && angle < 135) {
                    if (single_p)
                    {
                        points = 15;
                    }
                    else if (double_p)
                    {
                        points = 30;
                    }
                    else if (triple_p)
                    {
                        points = 45;
                    }
                }

                // Section 2
                else if (angle >= 135 && angle < 153) {
                    if (single_p)
                    {
                        points = 2;
                    }
                    else if (double_p)
                    {
                        points = 4;
                    }
                    else if (triple_p)
                    {
                        points = 8;
                    }
                }

                // Section 17
                else if (angle >= 153 && angle < 171) {
                    if (single_p)
                    {
                        points = 17;
                    }
                    else if (double_p)
                    {
                        points = 34;
                    }
                    else if (triple_p)
                    {
                        points = 51;
                    }
                }

                // Section 3
                else if (angle >= 171 && angle < 189) {
                    if (single_p)
                    {
                        points = 3;
                    }
                    else if (double_p)
                    {
                        points = 6;
                    }
                    else if (triple_p)
                    {
                        points = 9;
                    }
                }

                // Section 19
                else if (angle >= 189 && angle < 207) {
                    if (single_p)
                    {
                        points = 19;
                    }
                    else if (double_p)
                    {
                        points = 38;
                    }
                    else if (triple_p)
                    {
                        points = 57;
                    }
                }

                // Section 7
                else if (angle >= 207 && angle < 225) {
                    if (single_p)
                    {
                        points = 7;
                    }
                    else if (double_p)
                    {
                        points = 14;
                    }
                    else if (triple_p)
                    {
                        points = 21;
                    }
                }

                // Section 16
                else if (angle >= 225 && angle < 243) {
                    if (single_p)
                    {
                        points = 16;
                    }
                    else if (double_p)
                    {
                        points = 32;
                    }
                    else if (triple_p)
                    {
                        points = 48;
                    }
                }

                // Section 8
                else if (angle >= 243 && angle < 261) {
                    if (single_p)
                    {
                        points = 8;
                    }
                    else if (double_p)
                    {
                        points = 16;
                    }
                    else if (triple_p)
                    {
                        points = 24;
                    }
                }

                // Section 11
                else if (angle >= 261 && angle < 279) {
                    if (single_p)
                    {
                        points = 11;
                    }
                    else if (double_p)
                    {
                        points = 22;
                    }
                    else if (triple_p)
                    {
                        points = 33;
                    }
                }

                // Section 14
                else if (angle >= 279 && angle < 297) {
                    if (single_p)
                    {
                        points = 14;
                    }
                    else if (double_p)
                    {
                        points = 28;
                    }
                    else if (triple_p)
                    {
                        points = 32;
                    }
                }

                // Section 9
                else if (angle >= 297 && angle < 315) {
                    if (single_p)
                    {
                        points = 9;
                    }
                    else if (double_p)
                    {
                        points = 18;
                    }
                    else if (triple_p)
                    {
                        points = 27;
                    }
                }

                // Section 12
                else if (angle >= 315 && angle < 333) {
                    if (single_p)
                    {
                        points = 12;
                    }
                    else if (double_p)
                    {
                        points = 24;
                    }
                    else if (triple_p)
                    {
                        points = 36;
                    }
                }

                // Section 5
                else if (angle >= 333 && angle < 351) {
                    if (single_p)
                    {
                        points = 5;
                    }
                    else if (double_p)
                    {
                        points = 10;
                    }
                    else if (triple_p)
                    {
                        points = 15;
                    }
                }
            }
            total_points += points;
            String score_text = "Score: " + total_points.ToString();
            String dist_text = " Distance: " + d_radius; 
            String point_text = " Points: " + points;
            String throw_text = " Throws: " + total_throws.ToString();
            
            Debug.Log(score_text);
            TextMeshPro Scoreboard = (GameObject.FindWithTag("Scoreboard")).GetComponent<TextMeshPro>();
            Scoreboard.text = score_text + dist_text + point_text + throw_text;

        }
    }
   
}
