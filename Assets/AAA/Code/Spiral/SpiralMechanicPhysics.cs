using UnityEngine;
public class SpiralMechanicPhysics
{
    private readonly SpiralStats _stats;
    private readonly float _b;
    private readonly float _maxTheta;

    public SpiralMechanicPhysics(SpiralStats stats)
    {
        _stats = stats;
        _b = _stats.RadiusStep / (2f * Mathf.PI);
        _maxTheta = _stats.Turns * 2f * Mathf.PI;
    }

    public bool IsBulletCollidedWithSpiral(Bullet bullet)
    {
        Vector2 closestPoint = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        if (closestPoint == Vector2.positiveInfinity) return false;

        float distance = Vector2.Distance(bullet.Position, closestPoint);
        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        
        Vector2 vectorToSpiral = (closestPoint - bullet.Position).normalized;
        float dot = Vector2.Dot(bullet.Direction, vectorToSpiral);

        return distance <= collisionThreshold && dot > 0;
    }

    public void ReflectBullet(Bullet bullet)
    {
        Vector2 closestPointOnSpiral = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        if (closestPointOnSpiral == Vector2.positiveInfinity) return;

        Vector2 normal = (bullet.Position - closestPointOnSpiral).normalized;
        bullet.Direction = Vector2.Reflect(bullet.Direction.normalized, normal);

        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        float overlap = collisionThreshold - Vector2.Distance(bullet.Position, closestPointOnSpiral);
        if (overlap > 0)
        {
            bullet.Position += normal * (overlap + 0.01f);
        }
    }

    private Vector2 FindTrueClosestPointOnSpiral(Vector2 point, out float finalTheta)
    {
        // --- ПОЧАТОК ЄДИНОЇ ЗМІНИ ---

        // 1. Переходимо в локальні координати відносно центру спіралі
        Vector2 localPos = point - _stats.Center;

        // 2. "Розкручуємо" позицію кулі на кут, ОБЕРНЕНИЙ до повороту спіралі.
        //    Це дозволяє решті алгоритму працювати так, ніби спіраль не була повернута.
        float offsetRad = -_stats.AngleOffsetDeg * Mathf.Deg2Rad; // ВАЖЛИВО: ЗНАК МІНУС
        float cos = Mathf.Cos(offsetRad);
        float sin = Mathf.Sin(offsetRad);
        
        Vector2 unrotatedLocalPos = new Vector2(
            localPos.x * cos - localPos.y * sin,
            localPos.x * sin + localPos.y * cos
        );

        // 3. Використовуємо цю "розкручену" позицію для початкової оцінки theta
        float r = unrotatedLocalPos.magnitude;
        
        // --- КІНЕЦЬ ЗМІНИ. РЕШТА КОДУ ІДЕНТИЧНА ВАШОМУ РОБОЧОМУ ВАРІАНТУ ---

        float thetaEstimate = 0f;
        if (_b != 0)
        {
            thetaEstimate = (r - _stats.RadiusStart) / _b;
        }
        
        thetaEstimate = Mathf.Clamp(thetaEstimate, 0, _maxTheta);
        
        float searchTheta = thetaEstimate;
        int iterations = 10;

        for (int i = 0; i < iterations; i++)
        {
            Vector2 spiralPoint = GetSpiralPoint(searchTheta);
            Vector2 tangent = GetSpiralTangent(searchTheta);

            if (tangent.sqrMagnitude < 0.0001f) break;
            
            // ВАЖЛИВО: тут ми порівнюємо реальну позицію кулі 'point'
            // з реальною позицією точки на повернутій спіралі 'spiralPoint'.
            float dot = Vector2.Dot(point - spiralPoint, tangent);
            
            searchTheta += dot / tangent.sqrMagnitude;

            if (searchTheta < 0 || searchTheta > _maxTheta)
            {
                searchTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);
                break;
            }
        }
        
        finalTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);

        Vector2 finalPoint = GetSpiralPoint(finalTheta);
        float maxRadius = _stats.RadiusStart + _b * _maxTheta + _stats.SpiralThickness;
        if (localPos.magnitude > maxRadius * 2) 
        {
            return Vector2.positiveInfinity;
        }

        return finalPoint;
    }

    private Vector2 GetSpiralPoint(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;

        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle)) + _stats.Center;
    }
    
    private Vector2 GetSpiralTangent(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float dr_dtheta = _b;

        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;
        float d_angle_d_theta = _stats.Clockwise ? -1f : 1f;

        float cos_a = Mathf.Cos(angle);
        float sin_a = Mathf.Sin(angle);

        float dx_dtheta = dr_dtheta * cos_a - r * sin_a * d_angle_d_theta;
        float dy_dtheta = dr_dtheta * sin_a + r * cos_a * d_angle_d_theta;
        
        return new Vector2(dx_dtheta, dy_dtheta);
    }
}
    /* Версія де все добре, але не враховано кут спіралі
public class SpiralMechanicPhysics
{
    private readonly SpiralStats _stats;
    private readonly float _b;
    private readonly float _maxTheta;

    public SpiralMechanicPhysics(SpiralStats stats)
    {
        _stats = stats;
        _b = _stats.RadiusStep / (2f * Mathf.PI);
        _maxTheta = _stats.Turns * 2f * Mathf.PI;
    }

    public bool IsBulletCollidedWithSpiral(Bullet bullet)
    {
        Vector2 closestPoint = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        
        // Якщо точка не знайдена (куля далеко за межами спіралі), колізії немає
        if (closestPoint == Vector2.positiveInfinity) return false;

        float distance = Vector2.Distance(bullet.Position, closestPoint);
        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        
        Vector2 vectorToSpiral = (closestPoint - bullet.Position).normalized;
        float dot = Vector2.Dot(bullet.Direction, vectorToSpiral);

        return distance <= collisionThreshold && dot > 0;
    }

    public void ReflectBullet(Bullet bullet)
    {
        Vector2 closestPointOnSpiral = FindTrueClosestPointOnSpiral(bullet.Position, out _);
        
        if (closestPointOnSpiral == Vector2.positiveInfinity) return;

        Vector2 normal = (bullet.Position - closestPointOnSpiral).normalized;
        bullet.Direction = Vector2.Reflect(bullet.Direction.normalized, normal);

        float collisionThreshold = bullet.Radius + (_stats.SpiralThickness / 2f);
        float overlap = collisionThreshold - Vector2.Distance(bullet.Position, closestPointOnSpiral);
        if (overlap > 0)
        {
            bullet.Position += normal * (overlap + 0.01f);
        }
    }

    /// <summary>
    /// Знаходить найближчу точку, використовуючи надійну початкову оцінку та ітеративне уточнення.
    /// </summary>
    private Vector2 FindTrueClosestPointOnSpiral(Vector2 point, out float finalTheta)
    {
        // --- КРОК 1: НАДІЙНА ПОЧАТКОВА ОЦІНКА THETA ---
        Vector2 localPos = point - _stats.Center;
        float r = localPos.magnitude;
        
        // Оцінюємо theta з радіуса, щоб отримати правильний "виток"
        // Це головне виправлення - найнадійніший спосіб оцінити, як далеко по спіралі ми є
        float thetaEstimate = 0f;
        if (_b != 0)
        {
            thetaEstimate = (r - _stats.RadiusStart) / _b;
        }

        // Обмежуємо оцінку в межах спіралі
        thetaEstimate = Mathf.Clamp(thetaEstimate, 0, _maxTheta);

        // --- КРОК 2: УТОЧНЕННЯ ---
        // Тепер, маючи гарну початкову точку, запускаємо цикл уточнення.
        float searchTheta = thetaEstimate;
        int iterations = 10; // Дамо більше ітерацій для гарантії

        for (int i = 0; i < iterations; i++)
        {
            // В істинно найближчій точці, вектор від спіралі до кулі перпендикулярний дотичній.
            // Отже, їх скалярний добуток (dot product) дорівнює нулю.
            // Ми шукаємо таке `searchTheta`, щоб dot( (point - S(theta)), S'(theta) ) = 0
            Vector2 spiralPoint = GetSpiralPoint(searchTheta);
            Vector2 tangent = GetSpiralTangent(searchTheta);

            // Проста перевірка на випадок нульової дотичної
            if (tangent.sqrMagnitude < 0.0001f) break;
            
            float dot = Vector2.Dot(point - spiralPoint, tangent);

            // Використовуємо метод Ньютона для швидкого пошуку кореня (де dot=0)
            // theta_new = theta_old - f(theta) / f'(theta)
            // f'(theta) тут є складною функцією, але ми можемо її апроксимувати
            // як -tangent.sqrMagnitude. Це дає стабільний і швидкий результат.
            searchTheta += dot / tangent.sqrMagnitude;

            // Тримаємо theta в межах спіралі
            if (searchTheta < 0 || searchTheta > _maxTheta)
            {
                searchTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);
                break;
            }
        }
        
        finalTheta = Mathf.Clamp(searchTheta, 0, _maxTheta);

        // Якщо куля занадто далеко, можемо повернути "нескінченність", щоб уникнути дивної поведінки
        Vector2 finalPoint = GetSpiralPoint(finalTheta);
        float maxRadius = _stats.RadiusStart + _b * _maxTheta + _stats.SpiralThickness;
        if (r > maxRadius * 2) // Якщо куля дуже далеко
        {
            return Vector2.positiveInfinity;
        }

        return finalPoint;
    }

    private Vector2 GetSpiralPoint(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;

        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle)) + _stats.Center;
    }
    
    private Vector2 GetSpiralTangent(float theta)
    {
        float r = _stats.RadiusStart + _b * theta;
        float dr_dtheta = _b;

        float angle = _stats.AngleOffsetDeg * Mathf.Deg2Rad;
        angle += _stats.Clockwise ? -theta : theta;
        float d_angle_d_theta = _stats.Clockwise ? -1f : 1f;

        float cos_a = Mathf.Cos(angle);
        float sin_a = Mathf.Sin(angle);

        float dx_dtheta = dr_dtheta * cos_a - r * sin_a * d_angle_d_theta;
        float dy_dtheta = dr_dtheta * sin_a + r * cos_a * d_angle_d_theta;
        
        return new Vector2(dx_dtheta, dy_dtheta);
    }
}
*/