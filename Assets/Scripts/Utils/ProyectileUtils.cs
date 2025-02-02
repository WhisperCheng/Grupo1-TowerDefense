using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ProyectileUtils
{
    // Para hacer la trayerctoria dada una velocidad inicial (Este es el caso que se va a usar):
    //https://stackoverflow.com/questions/30290262/how-to-throw-a-ball-to-a-specific-point-on-plane
    //https://discussions.unity.com/t/input-force-is-nan-nan-nan/190950
    // Para hacer la trayectoria dada una altura máxima (lo dejo como apunte por si lo necesito
    // yo en el futuro, no lo voy a usar):
    //https://youtu.be/IvT8hjy6q4o
    public static bool ThrowProyectileAtLocation(Transform thrower, GameObject ballGameObject, Vector3 targetLocation, float initialShotSpeed)
    {
        bool result = true;
        Vector3 direction = (targetLocation - thrower.position).normalized;
        float distance = Vector3.Distance(targetLocation, thrower.position);

        Vector3 anguloSueloPos = new Vector3(targetLocation.x, thrower.position.y, targetLocation.z);
        Vector3 centroPos = thrower.position;
        Vector3 directionA = Vector3.Normalize(centroPos - anguloSueloPos);
        Vector3 directionB = Vector3.Normalize(centroPos - targetLocation);
        // https://stackoverflow.com/questions/49383884/find-angle-between-two-objects-while-taking-another-object-as-center-in-unity-us

        float angulo = Vector3.Angle(directionA, directionB) * (thrower.position.y > targetLocation.y ? -1 : 1);

        float firingElevationAngle = FiringElevationAngle(Physics.gravity.magnitude, distance, initialShotSpeed);

        if (firingElevationAngle == -1f)
        {
            return false; // Se sale si el ángulo es inválido y no dispara
        }

        firingElevationAngle = 90f - firingElevationAngle - angulo;

        Vector3 elevation = Quaternion.AngleAxis(firingElevationAngle, thrower.right) * Vector3.up / 2;
        float directionAngle = AngleBetweenAboutAxis(thrower.forward, direction, thrower.up);
        Vector3 velocity = Quaternion.AngleAxis(directionAngle, thrower.up) * elevation * initialShotSpeed * 2;

        // ballGameObject is object to be thrown
        ballGameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

        return result;
    }

    /*public static bool test(Transform thrower, GameObject ballGameObject, Vector3 targetLocation, float initialShotSpeed)
    {
        ballGameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
        return true;
    }*/

    // Helper method to find angle between two points (v1 & v2) with respect to axis n
    private static float AngleBetweenAboutAxis(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    // Helper method to find angle of elevation (ballistic trajectory) required to reach distance with initialVelocity
    // Does not take wind resistance into consideration.

    /*  
     *  Si el destino a apuntar está fuera de rango (es decir, si con la velocidad inicial designada
     *  no es posible alcanzar el objetivo porque está muy lejos) el try dará un error, por lo que en ese
     *  caso se usará un ángulo de -1º para indicar que no se disparará
     */
    private static float FiringElevationAngle(float gravity, float distance, float initialVelocity)
    {
        float angle = -1f; // Angulo por defecto
        try
        {
            angle = 0.5f * Mathf.Asin((gravity * distance) / (initialVelocity * initialVelocity))
            * Mathf.Rad2Deg;
            // Calcula el ángulo en base a la velocidad inicial, la gravedad y la distancia.
            // Si no es posible llegar a alcanzar el destino dada la velocidad incial del proyectil y la gravedad, se intentaría
            // calcular el arcoseno de un ángulo cuyo seno sería superior a 1 o inferior a -1, lo cual no es posible, por lo que
            // daría un error y angle seguiría siendo -1f, cuyo valor se usará para indicar que el objetivo está fuera de rango
        }
        catch (Exception)
        {

        }
        if (float.IsNaN(angle))
        {
            angle = -1f; // Si está fuera de rango entonces se pone a -1 para indicar que no se debe disparar
        }
        return angle;
    }

    public class ShootingInterception
    { // Referencia:  http://wiki.unity3d.com/index.php/Calculating_Lead_For_Projectiles
        public static Vector3 CalculateInterceptionPoint(float initialShotVelocity, Transform shooter, Transform target,
            Vector3 offsetTargetPosition)
        {
            // === variables you need ===
            //how fast our shots move -> initialShotVelocity
            //objects
            //Rigidbody shooterRb = thrower.GetComponent<Rigidbody>();
            NavMeshAgent targetNavm = target.GetComponent<NavMeshAgent>();

            // === derived variables ===
            //positions
            Vector3 shooterPosition = shooter.transform.position;
            Vector3 targetPosition = target.transform.position + offsetTargetPosition;
            //velocities
            // Las torres van a estar siempre quietas, pero si se estuvieran moviendo entonces habría que coger su rigidbody
            // y tener en cuenta su velocidad -> shooterVelocity = shooterRb ? shooterRb.velocity : Vector3.zero; 
            Vector3 shooterVelocity = Vector3.zero; 
            Vector3 targetVelocity = targetNavm ? targetNavm.velocity : Vector3.zero;

            //calculate intercept
            return FirstOrderIntercept(shooterPosition, shooterVelocity, initialShotVelocity, targetPosition, targetVelocity);
        }
        

        //first-order intercept using absolute target position
        private static Vector3 FirstOrderIntercept
        (Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime (shotSpeed, targetRelativePosition, targetRelativeVelocity);
            return targetPosition + t * (targetRelativeVelocity);
        }
        //first-order intercept using relative target position
        private static float FirstOrderInterceptTime
        (float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f)
                return 0f;

            float a = velocitySquared - shotSpeed * shotSpeed;

            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
                return Mathf.Max(t, 0f); //don't shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f)
            { //determinant > 0; two intercept paths (most common)
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                        t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f)
                {
                    if (t2 > 0f)
                        return Mathf.Min(t1, t2); //both are positive
                    else
                        return t1; //only t1 is positive
                }
                else
                    return Mathf.Max(t2, 0f); //don't shoot back in time
            }
            else if (determinant < 0f) //determinant < 0; no intercept path
                return 0f;
            else //determinant = 0; one intercept path, pretty much never happens
                return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }
        //now use whatever method to launch the projectile at the intercept point
    }
}
