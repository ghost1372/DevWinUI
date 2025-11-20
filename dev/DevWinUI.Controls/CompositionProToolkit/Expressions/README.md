<p align="center">
<img align="center" src="https://cloud.githubusercontent.com/assets/7021835/18138158/41f5d4aa-6f60-11e6-8373-9e1085130bff.png" alt="CompositionProToolkit.Expressions logo">
</p>

# 1. Contents
<!-- TOC -->

- [1. Contents](#1-contents)
- [2. CompositionAnimation](#2-compositionanimation)
- [3. CompositionProToolkit.Expressions](#3-compositionprotoolkitexpressions)
- [4. Construction of Expressions](#4-construction-of-expressions)
- [5. CompositionExpression&lt;T&gt;](#5-compositionexpressiont)
- [6. CompositionExpressionContext&lt;T&gt;](#6-compositionexpressioncontextt)
- [7. Expression Keywords](#7-expression-keywords)
    - [7.1. Example](#71-example)
- [8. Using `new` operator in the Expression](#8-using-new-operator-in-the-expression)
    - [8.1. Example](#81-example)
- [9. Expression Template](#9-expression-template)
    - [9.1. Expression Target](#91-expression-target)
    - [9.2. Example](#92-example)
    - [9.3. Expression Reference](#93-expression-reference)
    - [9.4. Example](#94-example)
- [10. Parameters](#10-parameters)
    - [10.1. Structural Parameters](#101-structural-parameters)
    - [10.2. Reference Parameters](#102-reference-parameters)
    - [10.3. Example](#103-example)
- [11. Using CompositionPropertySet within the Expression](#11-using-compositionpropertyset-within-the-expression)
    - [11.1. Example](#111-example)
- [12. Scalar Constants](#12-scalar-constants)
- [13. Defining the ExpressionAnimation&lt;T&gt;](#13-defining-the-expressionanimationt)
- [14. Using Lambda Expressions for `StartAnimation` & `StopAnimation`](#14-using-lambda-expressions-for-startanimation--stopanimation)
    - [14.1. Example](#141-example)
        - [14.1.1. Without using lambda expressions](#1411-without-using-lambda-expressions)
        - [14.1.2. Using lambda expressions](#1412-using-lambda-expressions)
- [15. Using Arrays in Expression](#15-using-arrays-in-expression)
    - [15.1. Example](#151-example)
- [16. Using List&lt;&gt; in Expression](#16-using-list-in-expression)
    - [16.1. Example](#161-example)
- [17. Using Dictionary&lt;,&gt; in Expression](#17-using-dictionary-in-expression)
    - [17.1. Example](#171-example)
- [18. Optimizing the KeyFrameAnimation](#18-optimizing-the-keyframeanimation)
    - [18.1. KeyFrame&lt;T&gt;](#181-keyframet)
    - [18.2. KeyFrameAnimation&lt;T&gt;](#182-keyframeanimationt)
    - [18.3. Example](#183-example)
        - [18.3.1. Without using CompositionProToolkit.Expressions](#1831-without-using-compositionprotoolkitexpressions)
        - [18.3.2. Using CompositionProToolkit.Expressions](#1832-using-compositionprotoolkitexpressions)
- [19. Custom Cubic Bezier Easing Functions](#19-custom-cubic-bezier-easing-functions)
- [20. APPENDIX A](#20-appendix-a)
- [21. CompositionExpressionContext&lt;T&gt;](#21-compositionexpressioncontextt)
    - [21.1. CompositionExpressionContext&lt;T&gt; Properties](#211-compositionexpressioncontextt-properties)
    - [21.2. CompositionExpressionContext&lt;T&gt; Functions](#212-compositionexpressioncontextt-functions)
        - [21.2.1. Scalar Functions](#2121-scalar-functions)
        - [21.2.2. Vector2 Functions](#2122-vector2-functions)
        - [21.2.3. Vector3 Functions](#2123-vector3-functions)
        - [21.2.4. Vector4 Functions](#2124-vector4-functions)
        - [21.2.5. Matrix3x2 Functions](#2125-matrix3x2-functions)
        - [21.2.6. Matrix4x4 Functions](#2126-matrix4x4-functions)
        - [21.2.7. Quaternion Functions](#2127-quaternion-functions)
        - [21.2.8. Color Functions](#2128-color-functions)
        - [21.2.9. Expression Templates - Targets](#2129-expression-templates---targets)
        - [21.2.10. Expression Templates - References](#21210-expression-templates---references)

<!-- /TOC -->

# 2. CompositionAnimation

Composition animations provide a powerful and efficient way to run animations in your application UI. They ensure that your animations run at 60 FPS independent of the UI thread.
There are two types of Composition Animations: **KeyFrame Animations**, and **Expression Animations**.

**KeyFrame Animations** are time bound animations in which the developer can explicitly define the control points describing values an animating property needs to be at specific points in the animation timeline. The developer can also influence the interpolation of values between two control points by specifying the easing function.

**ExpressionAnimations** allow a developer to define a mathematical equation that can be used to calculate the value of a targeted animating property each frame. The mathematical equation can be defined using references to properties of Composition objects, mathematical functions and operators and Input. Expression Animations open the door to making experiences such as sticky headers and parallax easily describable.

The real power of Expression Animations comes from their ability to create a mathematical relationship with references to properties on other objects. This means you can have an equation referencing property values on other Composition objects, local variables, or even shared values in Composition Property Sets. As these references change and update over time, your expression will as well. This opens up bigger possibilities beyond traditional KeyFrame Animations where values must be discrete and pre-defined ExpressionAnimations can make more dynamic animation experiences.

To use ExpressionAnimations today, developers are required to write their mathematical equation/relationship in a string (example shown below). 

```C#
_parallaxExpression = compositor.CreateExpressionAnimation();
_parallaxExpression.Expression = "(ScrollManipulation.Translation.Y + 
    StartOffset - (0.5 * ItemHeight)) * ParallaxValue - 
    (ScrollManipulation.Translation.Y + StartOffset - (0.5 * ItemHeight))";
```

This experience presents a series of challenges for developers:
- No IntelliSense or auto complete support.
- No type safety when building equations.
- All errors are runtime errors, many of which are desirable to be detected at compile time. 
- Working with strings for complicated equations not intuitive or ideal.

In order to improve the Expression Authoring experience, you can use the **CompositionProToolkit** library.

# 3. CompositionProToolkit.Expressions

**CompositionProToolkit.Expressions** namespace provides a set of helper classes and extension methods which facilitate the developer to define the mathematical equation in the form of a lambda expression. These helper classes and extension methods facilitate parsing the lambda expression using Expression Trees and converts it into an appropriate expression string. Thus it provides type-safety, IntelliSense support and allows catching of errors during compile time.
In this document, the mathematical equations used in ExpressionAnimations will be referred to as an **Expression**.

# 4. Construction of Expressions
While defining Expressions, you must pay attention to the type of the property you plan to animate – your equation must resolve to the same type. Otherwise, an error will get thrown when the expression gets calculated. If your equation resolves to Nan (number/0), the system will use the previously calculated value. The mathematical equation will be used every frame to calculate the value of the animating property.
Expression animations can be applied to most properties of Composition objects such as Visual or property sets. Most of these properties are of one of the following types:
- `Scalar`
- `Vector2`
- `Vector3`
- `Vector4`
- `Color`
- `Quaternion`

# 5. CompositionExpression&lt;T&gt;

Using the helper classes and extension methods defined in CompositionProToolkit.Expressions namespace, Expressions are defined as lambda expressions. The signature of the lamba expression is represented by the **CompositionExpression&lt;T&gt;** delegate. It is defined as

```C#
public delegate T CompositionExpression<T>(CompositionExpressionContext<T> ctx);
```

_**Here, T represents the type of the targeted animating property of a composition object.**_

# 6. CompositionExpressionContext&lt;T&gt;

The **CompositionExpression&lt;T&gt;** delegate requires an argument of type **CompositionExpressionContext&lt;T&gt;**. This generic class provides a set of properties and functions which can be used to construct the Expression. These include mathematical functions and functions to construct objects.
The lambda expression will be defined as follows (say you want to animate a Vector2 property)

```C#
Expression<CompositionExpression<Vector2>> expr = c => new Vector2(c.Cos(20), c.Sin(20));
```

In the above lambda expression, c represents an object of type `CompositionExpressionContext<Vector2>` and you can use any of its functions to define your Expression.

_The full list of functions available in the **CompositionExpressionContext&lt;T&gt;** is provided at the end of this document ([Appendix A](#20-appendix-a))._

# 7. Expression Keywords
The CompositionExpressionContext&lt;T&gt; class provides the following properties which are evaluated as keywords within the Expression

| Property | Description |
| ------- | ------- |
| **StartingValue** | Provides a reference to the original starting value of the property that is being animated.|
| **CurrentValue** | Provides a reference to the currently “known” value of the property |
| **FinalValue** | Provides a reference to the final value of the animation (if defined) Note: Relevant for Implicit Animations, for explicit, maintains same functionality as StartingValue |

Within the Expression, these properties will resolve to the type of the animated property. 
 
## 7.1. Example

```C#
var rotationAnimation = _compositor.GenerateScalarKeyFrameAnimation()
                                   .HavingDuration(TimeSpan.FromSeconds(3))
                                   .RepeatsForever();

var easingFn = _compositor.CreateLinearEasingFunction();

rotationAnimation.InsertExpressionKeyFrame(1, 
                                           c => c.StartingValue + Float.TwoPi,
                                           easingFn);
visual1.StartAnimation(() => visual1.RotationAngle, rotationAnimation);
```

# 8. Using `new` operator in the Expression
You can use the new operator within the Expression body to construct new objects. Right now the new operator is supported for the following constructors

```C#
Vector2(float x, float y);
Vector3(float x, float y, float z);
Vector4(float x, float y, float z, float w);
Matrix3x2(float m11, float m12, 
          float m21, float m22, 
          float m31, float m32)
Matrix4x4(float m11, float m12, float m13, float m14, 
          float m21, float m22, float m23, float m24, 
          float m31, float m32, float m33, float m34, 
          float m41, float m42, float m43, float m44);
Quaternion(float x, float y, float z, float w);
```

The new operator can also be used to create **ExpressionTarget** and **ExpressionReference** objects. (_More on that in the following section._)

## 8.1. Example

```C#
Visual visual = _compositor.CreateSpriteVisual();
Expression<CompositionExpression<Vector3>> expr =
     c => visual.Offset * 0.5f + new Vector3(10, 20, 30);
```

# 9. Expression Template
All composition animations in the Visual Layer are _templates_ – this means that developers can use an animation on multiple objects without the need to create separate animations. This allows developers to use the same animation and tweak properties or parameters to meet some other needs without the worry of obstructing the previous uses.
## 9.1. Expression Target
**ExpressionTarget** creates a reference to CompositionObject being targeted by the Expression. This allows you to use the obtained object’s properties within the Expression.
Currently the following ExpressionTargets are supported (the name in parenthesis refers to the actual Composition object being targeted)
- `AmbientLightTarget` (_**`AmbientLight`**_)
- `ColorBrushTarget` (_**`CompositionColorBrush`**_)
- `DistantLightTarget` (_**`DistantLight`**_)
- `DropShadowTarget` (_**`DropShadow`**_)
- `InsetClipTarget` (_**`InsetClip`**_)
- `InteractionTrackerTarget` (_**`InteractionTracker`**_)
- `ManipulationPropertySetTarget` (_**`CompositionPropertySet`**_)
- `NineGridBrushTarget` (_**`CompositionNineGridBrush`**_)
- `PointerPositionPropertySetTarget` (_**`CompositionPropertySet`**_)
- `PointLightTarget` (_**`PointLight`**_)
- `SpotLightTarget` (_**`SpotLight`**_)
- `SurfaceBrushTarget` (_**`CompositionSurfaceBrush`**_)
- `VisualTarget` (_**`Visual`**_)

## 9.2. Example

```C#
// windowWidth defined earlier
var opacityAnimation = _compositor.CreateScalarExpressionAnimation();
opacityAnimation.Expression = c => new VisualTarget().Offset.X / windowWidth;
_visual.StartAnimation(() => _visual.Opacity, opacityAnimation);
```

## 9.3. Expression Reference
**ExpressionReference** allows you to create an alias for a Composition object and use that alias within the Expression. The biggest advantage of this is that you can reuse your ExpressionAnimation on different composition objects by providing different values to the alias.
Currently the following ExpressionReferences are supported (the name in parenthesis refers to the actual Composition object being referenced)
- `AmbientLightReference` (_**`AmbientLight`**_)
- `ColorBrushReference` (_**`CompositionColorBrush`**_)
- `DistantLightReference` (_**`DistantLight`**_)
- `DropShadowReference` (_**`DropShadow`**_)
- `InsetClipReference` (_**`InsetClip`**_)
- `InteractionTrackerReference` (_**`InteractionTracker`**_)
- `ManipulationPropertySetReference` (_**`CompositionPropertySet`**_)
- `NineGridBrushReference` (_**`CompositionNineGridBrush`**_)
- `PointerPositionPropertySetReference` (_**`CompositionPropertySet`**_)
- `PointLightReference` (_**`PointLight`**_)
- `SpotLightReference` (_**`SpotLight`**_)
- `SurfaceBrushReference` (_**`CompositionSurfaceBrush`**_)
- `VisualReference` (_**`Visual`**_)

To connect a composition object to the ExpressionReference defined in the Expression, use the `SetReference()` method of **ExpressionAnimation&lt;T&gt;**.

## 9.4. Example

```C#
var offsetAnimation = _compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = 
    c => new VisualReference("myVisual").CenterPoint + new Vector3(10, 20, 30) + delta;
offsetAnimation.SetReference("myVisual", visual);

visual.StartAnimation(() => visual.Offset, offsetAnimation);

var visual2 = _compositor.CreateSpriteVisual();
visual2.Offset = new Vector3(10, 10, 10);
visual2.Size = new Vector2(100, 100);

offsetAnimation.SetReference("myVisual", visual2);

visual2.StartAnimation(() => visual2.Offset, offsetAnimation);
```

# 10. Parameters
## 10.1. Structural Parameters
Structural parameters are objects which are locally created and used within the Expression. Their alias name within the Expression is the same as the name of the object. Using the `SetReference()` method you can use this alias name to update the references of these parameters.

```C#
var visual = _compositor.CreateSpriteVisual();
visual.Offset = new Vector3(20, 30, 40);
visual.Size = new Vector2(200, 100);

var delta = new Vector3(50);

var offsetAnimation = _compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = 
    c => visual.CenterPoint + new Vector3(10, 20, 30) + delta;

visual.StartAnimation(() => visual.Offset, offsetAnimation);

var visual2 = _compositor.CreateSpriteVisual();
visual2.Offset = new Vector3(10, 10, 10);
visual2.Size = new Vector2(100, 100);

offsetAnimation.SetReference("visual", visual2);
offsetAnimation.SetReference("delta", new Vector3(100));

visual2.StartAnimation(() => visual2.Offset, offsetAnimation);
```

In the above example, the Expression has two parameters defined: `visual` and `delta`. Using these aliases, the reference to their parameters are updated to new values.

## 10.2. Reference Parameters
Reference Parameters are objects created explicitly by creating `ExpressionReference` objects (either via the `new` operator or using corresponding functions in CompositionExpressionContext&lt;T&gt;). The alias name for these parameters are provided by the developer and their references can be updated using the `SetReference()` method.
 
## 10.3. Example

```C#
var visual = _compositor.CreateSpriteVisual();
visual.Offset = new Vector3(20, 30, 40);
visual.Size = new Vector2(200, 100);

var delta = new Vector3(50);

var offsetAnimation = _compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = 
    c => new VisualReference("myVisual").CenterPoint + new Vector3(10, 20, 30) + delta;
offsetAnimation.SetReference("myVisual", visual);

visual.StartAnimation(() => visual.CenterPoint, offsetAnimation);

var visual2 = _compositor.CreateSpriteVisual();
visual2.Offset = new Vector3(10, 10, 10);
visual2.Size = new Vector2(100, 100);

offsetAnimation.SetReference("myVisual", visual2);
offsetAnimation.SetReference("delta", new Vector3(100));

visual2.StartAnimation(() => visual2.CenterPoint, offsetAnimation);
```


# 11. Using CompositionPropertySet within the Expression
In order to use a **CompositionPropertySet** within the Expression the following `GetXXX` extension methods have been defined

```C#
public static bool GetBoolean(this CompositionPropertySet propertySet, string key);
public static Color GetColor(this CompositionPropertySet propertySet, string key);
public static Matrix3x2 GetMatrix3x2(this CompositionPropertySet propertySet, string key);
public static Matrix4x4 GetMatrix4x4(this CompositionPropertySet propertySet, string key);
public static Quaternion GetQuaternion(this CompositionPropertySet propertySet, string key);
public static float GetScalar(this CompositionPropertySet propertySet, string key);
public static Vector2 GetVector2(this CompositionPropertySet propertySet, string key);
public static Vector3 GetVector3(this CompositionPropertySet propertySet, string key);
public static Vector4 GetVector4(this CompositionPropertySet propertySet, string key);
```

## 11.1. Example

```C#
Point position = new Point(0, 0);
Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
CompositionPropertySet scrollProperties =
    ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

position.X += scrollViewer.HorizontalOffset;
position.Y += scrollViewer.VerticalOffset;
var itemOffset = new Vector3(position.X.ToSingle(), position.Y.ToSingle(), 0);

offsetAnimation.Duration = TimeSpan.FromSeconds(1);

// Create the CompositionLambda Expression
Expression<CompositionExpression<Vector3>> expression =
    c => c.Vector3(scrollProperties.GetVector2("Translation").X,
                scrollProperties.GetVector2("Translation").Y, 0) + itemOffset;

// Set the Expression
offsetAnimation.InsertExpressionKeyFrame(1f, expression);
```

# 12. Scalar Constants
The static class **Scalar** defines several constants related to `Math.PI` as floating point numbers. They can be used within the Expression. It also contains two conversion factors to convert from radians to degrees and vice versa.

- **`Scalar.Pi`** - same as `(float)Math.PI` radians ( or **180 degrees**).
- **`Scalar.TwoPi`** - Two times `(float)Math.PI` radians ( or **360 degrees**).
- **`Scalar.PiByTwo`** - half of `(float)Math.PI` radians ( or **90 degrees**).
- **`Scalar.PiByThree`** - one third of `(float)Math.PI` radians ( or **60 degrees**).
- **`Scalar.PiByFour`** - one fourth of `(float)Math.PI` radians ( or **45 degrees**).
- **`Scalar.PiBySix`** - one sixth of `(float)Math.PI` radians ( or **30 degrees**).
- **`Scalar.ThreePiByTwo`** - three times half of `(float)Math.PI` radians ( or **270 degrees**).
- **`Scalar.DegreesToRadians`** - 1 degree in radians.
- **`Scalar.RadiansToDegrees`** - 1 radian in degrees.

# 13. Defining the ExpressionAnimation&lt;T&gt;

The following extension methods have been defined on the **Compositor** to create the appropriate `ExpressionAnimation<T>` object.

```C#
public static ExpressionAnimation<Color> CreateColorExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<Quaternion> CreateQuaternionExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<float> CreateScalarExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<Vector2> CreateVector2ExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<Vector3> CreateVector3ExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<Vector4> CreateVector4ExpressionAnimation(this Compositor compositor);
public static ExpressionAnimation<Matrix4x4> CreateMatrix4x4ExpressionAnimation(this Compositor compositor);
```

After creating the `ExpressionAnimation<T>` object you can set its `Expression` property with the appropriate Expression.

# 14. Using Lambda Expressions for `StartAnimation` & `StopAnimation`
In order to avoid providing the property names as string for `StartAnimation` and `StopAnimation` methods of CompositionObject, the following extension methods are provided

```C#
public static void StartAnimation(this CompositionObject compositionObject, 
    Expression<Func<object>> expression, CompositionAnimation animation);
public static void StartAnimation<T>(this CompositionObject compositionObject,
    Expression<Func<T>> expression, KeyFrameAnimation<T> keyframeAnimation);
public static void StartAnimation<T>(this CompositionObject compositionObject,
    Expression<Func<T>> expression, ExpressionAnimation<T> expressionAnimation);
public static void StopAnimation(this CompositionObject compositionObject,
    Expression<Func<object>> expression);
public static string ScaleXY(this CompositionObject compositionObject);
```

_The method `ScaleXY` is a dummy method to specify that the animation has to be executed on both the `Scale.X` as well as the `Scale.Y` properties of CompositionObject simultaneously._

## 14.1. Example 
### 14.1.1. Without using lambda expressions

```C#
rootVisual.StartAnimation("Opacity", fadeInAnimation);
rootVisual.StartAnimation("RotationAxis.X", rotateAnimation);
rootVisual.StartAnimation("Scale.XY", scaleAnimation);
rootVisual.StopAnimation("Offset", offsetAnimation);
```

### 14.1.2. Using lambda expressions

```C#
rootVisual.StartAnimation(() => rootVisual.Opacity, fadeInAnimation);
rootVisual.StartAnimation(() => rootVisual.RotationAxis.X, rotateAnimation);
rootVisual.StartAnimation(() => rootVisual.ScaleXY(), scaleAnimation);
rootVisual.StopAnimation(() => rootVisual.Offset, offsetAnimation);
```
# 15. Using Arrays in Expression
You can use an `Array` of objects deriving from CompositionObject within your Expression.

## 15.1. Example
```C#
var visual1 = compositor.CreateSpriteVisual();
...
var visual2 = compositor.CreateSpriteVisual();
...
var visualArray = new Visual[] { visual1, visual2 };

var offsetAnimation = compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = c => visualArray[0].Offset + new Vector(20);

visualArray[1].StartAnimation(() => visualArray[1].Offset, offsetAnimation);
```

# 16. Using List&lt;&gt; in Expression
You can use a `List<>` of objects deriving from CompositionObject within your Expression.

## 16.1. Example
```C#
var visual1 = compositor.CreateSpriteVisual();
...
var visual2 = compositor.CreateSpriteVisual();
...
var visualList = new List<Visual> { visual1, visual2 };

var offsetAnimation = compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = c => visualList[0].Offset + new Vector(20);

visualList[1].StartAnimation(() => visualList[1].Offset, offsetAnimation);
```

# 17. Using Dictionary&lt;,&gt; in Expression
You can use a `Dictionary<TKey, TValue>` within your Expression. 

**`TKey`** can be of types - `int`, `float`, `double` or `string`.
**`TValue`** should be an object deriving from `CompositionObject`.

## 17.1. Example
```C#
var visual1 = compositor.CreateSpriteVisual();
...
var visual2 = compositor.CreateSpriteVisual();
...
var visualDictionary = new Dictionary<string, Visual> 
    {
        ["first"] = visual1, 
        ["second"] = visual2 
    };

var offsetAnimation = compositor.CreateVector3ExpressionAnimation();
offsetAnimation.Expression = c => visualDictionary["first"].Offset + new Vector(20);

visualDictionary["second"].StartAnimation(() => visualDictionary["second"], offsetAnimation);
```

# 18. Optimizing the KeyFrameAnimation
## 18.1. KeyFrame&lt;T&gt;
**KeyFrame&lt;T&gt;** is a generic class which encapsulates the values required to define a KeyFrame within a `KeyFrameAnimation`. It has the following properties

| Property | Type | Description |
| ---------|------|------------ |
| Key | `float` | The time the key frame should occur at, expressed as a percentage of the animation Duration. Allowed value is from 0.0 to 1.0.
| Value | `T` | The type of the property being animated.
| Easing | `CompositionEasingFunction` | The easing function to use when interpolating between frames.

## 18.2. KeyFrameAnimation&lt;T&gt;
To construct a KeyFrame Animation, normally you use one of the constructor methods of the Compositor class that correlates to the structure type of the property you wish to animate.
- `CreateColorKeyFrameAnimation`
- `CreateQuaternionKeyFrameAnimation`
- `CreateScalarKeyFrameAnimation`
- `CreateVector2KeyFrameAnimation`
- `CreateVector3KeyFrameAnimation`
- `CreateVector4KeyFrameAnimation`

`KeyFrameAnimation<T>` is a generic class which encapsulates a `KeyFrameAnimation` object and provides a unified set of properties and methods which cater to the various animation classes deriving from KeyFrameAnimation. It has the following properties

| Property | Type | Description |
| ---------|------|------------ |
| Animation | `KeyFrameAnimation` | The encapsulated KeyFrameAnimation object.
| DelayTime | `TimeSpan` | The duration by which the animation should be delayed
| Direction | `AnimationDirection` | Direction of the Animation
| Duration | `TimeSpan` | The duration of the animation. Minimum allowed value is 1ms and maximum allowed value is 24 days.
| IterationBehavior | `AnimationIterationBehavior` | The iteration behavior for the key frame animation.
| IterationCount | `int` | The number of times to repeat the key frame animation. A value of -1 causes the animation to repeat indefinitely.
| KeyFrameCount | `int` | The number of key frames in the KeyFrameAnimation.
| StopBehavior | `AnimationStopBehavior` | Specifies how to set the property value when StopAnimation is called.
| Target | `String` | Specifies the target for the animation.

The following APIs facilitate the setting of keyframe(s) on the encapsulated KeyFrameAnimation object.

```C#
public void InsertKeyFrame(float normalizedProgressKey, T value, CompositionEasingFunction easingFunction = null);
public void InsertKeyFrame(KeyFrame<T> keyFrame);
public void InsertKeyFrames(params KeyFrame<T>[] keyFrames);
public void InsertExpressionKeyFrame(float normalizedProgressKey, Expression<CompositionExpression<T>> expression,
    CompositionEasingFunction easingFunction = null);
public void InsertStartingValueKeyFrame(float normalizedProgressKey, CompositionEasingFunction easingFunction = null);
public void InsertFinalValueKeyFrame(float normalizedProgressKey, CompositionEasingFunction easingFunction = null);
```

The `InsertStartingValueKeyFrame()` and `InsertFinalValueKeyFrame()` allow you to directly specify a **StartingValue** and **FinalValue** keyword respectively in the Expression at the specified keyframe using the specified easing function(if any).

The following APIs help you to set the properties of the encapsulated KeyFrameAnimation object. These method return the `KeyFrameAnimation<T>` objects itself (on which these methods are called), thus allowing these methods to be chained.

```C#
// Sets the Animation Direction of encapsulated KeyFrameAnimation object.
public KeyFrameAnimation<T> InTheDirection(AnimationDirection direction);

// Sets the duration by which the animation, defined by
public KeyFrameAnimation<T> DelayBy(TimeSpan delayTime);

// Sets the animation Duration of encapsulated KeyFrameAnimation object.
public KeyFrameAnimation<T> HavingDuration(TimeSpan duration);

// Sets the number of times the animation, defined by the encapsulated
// KeyFrameAnimation object, should be repeated.
public KeyFrameAnimation<T> Repeats(int count);

// Causes the animation defined by the encapsulated KeyFrameAnimation
// object to repeat indefinitely
public KeyFrameAnimation<T> RepeatsForever();

// Specifies how to set the property value when StopAnimation is called on
// the encapsulated KeyFrameAnimation object.
public KeyFrameAnimation<T> OnStop(AnimationStopBehavior stopBehavior);

// Sets the target for the encapsulated KeyFrameAnimation object
public KeyFrameAnimation<T> ForTarget(Expression<Func<T>> targetExpression);
```

The following extension method is defined for the `Compositor` to create a `KeyFrameAnimation<T>` object for the appropriate animating property

```C#
public static KeyFrameAnimation<Color> GenerateColorKeyFrameAnimation(this Compositor compositor);
public static KeyFrameAnimation<Quaternion> GenerateQuaternionKeyFrameAnimation(this Compositor compositor);
public static KeyFrameAnimation<float> GenerateScalarKeyFrameAnimation(this Compositor compositor);
public static KeyFrameAnimation<Vector2> GenerateVector2KeyFrameAnimation(this Compositor compositor);
public static KeyFrameAnimation<Vector3> GenerateVector3KeyFrameAnimation(this Compositor compositor);
public static KeyFrameAnimation<Vector4> GenerateVector4KeyFrameAnimation(this Compositor compositor);
```
 
## 18.3. Example
### 18.3.1. Without using CompositionProToolkit.Expressions
```C#
CubicBezierEasingFunction easeIn = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.0f, 0.51f),
                                                                               new Vector2(1.0f, 0.51f));
                                                                               
// Example 1
var enterAnimation = _compositor.CreateScalarKeyFrameAnimation();
enterAnimation.InsertKeyFrame(0.33f, 1.25f, easeIn);
enterAnimation.InsertKeyFrame(0.66f, 0.75f, easeIn);
enterAnimation.InsertKeyFrame(1.0f, 1.0f, easeIn);
enterAnimation.DelayTime = TimeSpan.FromMilliseconds(500);
enterAnimation.Duration = TimeSpan.FromMilliseconds(5000);
enterAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

spriteVisual.StartAnimation("Scale.XY", enterAnimation);

// Example 2
var exitAnimation = _compositor.CreateVector2KeyFrameAnimation();
exitAnimation.InsertKeyFrame(1.0f, new Vector2(0, 0));
exitAnimation.Duration = TimeSpan.FromMilliseconds(750);
exitAnimation.IterationBehavior = AnimationIterationBehavior.Count;
exitAnimation.IterationCount = 1;

spriteVisual2.StartAnimation("Offset", exitAnimation);

// Example 3 - ImplicitAnimations
var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
offsetAnimation.Target = "Offset";
offsetAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

var implicitAnimationCollection = _compositor.CreateImplicitAnimationCollection();
implicitAnimationCollection["Offset"] = offsetAnimation;

spriteVisual3.ImplicitAnimations = implicitAnimationCollection;
```

 
### 18.3.2. Using CompositionProToolkit.Expressions
```C#
CubicBezierEasingFunction easeIn = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.0f, 0.51f),
                                                                               new Vector2(1.0f, 0.51f));

// Example 1
var enterAnimation =_compositor.CreateKeyFrameAnimation<float>()
                               .HavingDuration(TimeSpan.FromSeconds(5))
                               .DelayBy(TimeSpan.FromMilliseconds(500))
                               .RepeatsForever();

enterAnimation.InsertKeyFrames(new KeyFrame<float>(0.33f, 1.25f, easeIn), 
                               new KeyFrame<float>(0.66f, 0.75f, easeIn), 
                               new KeyFrame<float>(1.0f, 1.0f, easeIn));
                               
spriteVisual.StartAnimation(() => spriteVisual.ScaleXY(), enterAnimation);

// Example 2
var exitAnimation = _compositor.CreateKeyFrameAnimation<Vector2>()
                               .HavingDuration(TimeSpan.FromMilliseconds(750))
                               .Repeats(1);

exitAnimation.InsertKeyFrame(1.0f, new Vector2(0, 0));

spriteVisual2.StartAnimation(() => spriteVisual2.Offset, exitAnimation);

// Example 3 - ImplicitAnimations
var offsetAnimation = _compositor.CreateKeyFrameAnimation<Vector3>()
                                 .HavingDuration(TimeSpan.FromMilliseconds(500))
                                 .ForTarget(() => spriteVisual3.Offset);

// Insert a 'this.FinalValue' expression at last keyframe                                 
offsetAnimation.InsertFinalValueKeyFrame(1f);

var implicitAnimationCollection = _compositor.CreateImplicitAnimationCollection();
implicitAnimationCollection["Offset"] = offsetAnimation.Animation;

spriteVisual3.ImplicitAnimations = implicitAnimationCollection;
```
# 19. Custom Cubic Bezier Easing Functions

The following extension methods have been added to `Compositor` to create predefined `CubicBezierEasingFunctions` (these custom cubic bezier easing functions are based on the [Robert Penner's Easing Equations](http://robertpenner.com/easing/) and the values are obtained from [Ceaser CSS Easing Animation Tool](https://matthewlein.com/ceaser/) )

```C#
public static CubicBezierEasingFunction CreateEaseInBackEasingFunction();
public static CubicBezierEasingFunction CreateEaseInCircleEasingFunction();
public static CubicBezierEasingFunction CreateEaseInCubicEasingFunction();
public static CubicBezierEasingFunction CreateEaseInExponentialFunction();
public static CubicBezierEasingFunction CreateEaseInQuadraticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInQuarticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInQuinticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInSineEasingFunction();

public static CubicBezierEasingFunction CreateEaseOutBackEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutCircleEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutCubicEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutExponentialFunction();
public static CubicBezierEasingFunction CreateEaseOutQuadraticEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutQuarticEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutQuinticEasingFunction();
public static CubicBezierEasingFunction CreateEaseOutSineEasingFunction();

public static CubicBezierEasingFunction CreateEaseInOutBackEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutCircleEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutCubicEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutExponentialFunction();
public static CubicBezierEasingFunction CreateEaseInOutQuadraticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutQuarticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutQuinticEasingFunction();
public static CubicBezierEasingFunction CreateEaseInOutSineEasingFunction();
```
# 20. APPENDIX A
# 21. CompositionExpressionContext&lt;T&gt;

## 21.1. CompositionExpressionContext&lt;T&gt; Properties

```
T StartingValue { get; }
T CurrentValue { get; }
T FinalValue { get; }
```
## 21.2. CompositionExpressionContext&lt;T&gt; Functions

### 21.2.1. Scalar Functions

```C#
public float Abs(float value);
public float Acos(float value);
public float Asin(float value);
public float Atan(float value);
public float Ceiling(float value);
public float Clamp(float value, float min, float max);
public float Cos(float value);
public float Distance(Vector2 value1, Vector2 value2);
public float Distance(Vector3 value1, Vector3 value2);
public float Distance(Vector4 value1, Vector4 value2);
public float DistanceSquared(Vector2 value1, Vector2 value2);
public float DistanceSquared(Vector3 value1, Vector3 value2);
public float DistanceSquared(Vector4 value1, Vector4 value2);
public float Floor(float value);
public float Length(Vector2 value);
public float Length(Vector3 value);
public float Length(Vector4 value);
public float Length(Quaternion value);
public float LengthSquared(Vector2 value);
public float LengthSquared(Vector3 value);
public float LengthSquared(Vector4 value);
public float LengthSquared(Quaternion value);
public float Ln(float value);
public float Log10(float value);
public float Max(float value1, float value2);
public float Min(float value1, float value2);
public float Mod(float dividend, float divisor);
public float Normalize();
public float Pow(float value, int power);
public float Round(float value);
public float Sin(float value);
public float Sqrt(float value);
public float Square(float value);
public float Tan(float value);
public float ToDegrees(float radians);
public float ToRadians(float degrees);
```
### 21.2.2. Vector2 Functions

```C#
public Vector2 Abs(Vector2 value);
public Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max);
public Vector2 Inverse(Vector2 value);
public Vector2 Lerp(Vector2 value1, Vector2 value2, float progress);
public Vector2 Max(Vector2 value1, Vector2 value2);
public Vector2 Min(Vector2 value1, Vector2 value2);
public Vector2 Normalize(Vector2 value);
public Vector2 Scale(Vector2 value, float factor);
public Vector2 Transform(Vector2 value, Matrix3x2 matrix);
public Vector2 Vector2(float x, float y);
```
### 21.2.3. Vector3 Functions

```C#
public Vector3 Abs(Vector3 value);
public Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max);
public Vector3 Inverse(Vector3 value);
public Vector3 Lerp(Vector3 value1, Vector3 value2, float progress);
public Vector3 Max(Vector3 value1, Vector3 value2);
public Vector3 Min(Vector3 value1, Vector3 value2);
public Vector3 Normalize(Vector3 value);
public Vector3 Scale(Vector3 value, float factor);
public Vector3 Vector3(float x, float y, float z);
```
### 21.2.4. Vector4 Functions

```C#
public Vector4 Abs(Vector4 value);
public Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max);
public Vector4 Max(Vector4 value1, Vector4 value2);
public Vector4 Min(Vector4 value1, Vector4 value2);
public Vector4 Inverse(Vector4 value);
public Vector4 Lerp(Vector4 value1, Vector4 value2, float progress);
public Vector4 Normalize(Vector4 value);
public Vector4 Scale(Vector4 value, float factor);
public Vector4 Transform(Vector4 value, Matrix4x4 matrix);
public Vector4 Vector4(float x, float y, float z, float w);
```
### 21.2.5. Matrix3x2 Functions

```C#
public Matrix3x2 CreateRotation(float radians);
public Matrix3x2 CreateScale(Vector2 scales);
public Matrix3x2 CreateSkew(float radiansX, float radiansY, Vector2 centerPoint);
public Matrix3x2 CreateTranslation(Vector2 position);
public Matrix3x2 Inverse(Matrix3x2 value);
public Matrix3x2 Lerp(Matrix3x2 value1, Matrix3x2 value2, float progress);
public Matrix3x2 Matrix3x2(float m11, float m12,
                           float m21, float m22,
                           float m31, float m32);
public Matrix3x2 Matrix3x2CreateFromScale(Vector2 scale);
public Matrix3x2 Matrix3x2CreateFromTranslation(Vector2 translation);
public Matrix3x2 Scale(Matrix3x2 value, float factor);
```
### 21.2.6. Matrix4x4 Functions

```C#
public Matrix4x4 CreateScale(Vector3 scale);
public Matrix4x4 CreateTranslation(Vector3 translation);
public Matrix4x4 Lerp(Matrix4x4 value1, Matrix4x4 value2, float progress);
public Matrix4x4 Matrix4x4(float m11, float m12, float m13, float m14,
                           float m21, float m22, float m23, float m24,
                           float m31, float m32, float m33, float m34,
                           float m41, float m42, float m43, float m44);
public Matrix4x4 Matrix4x4(Matrix3x2 matrix);
public Matrix4x4 Matrix4x4CreateFromScale(Vector3 scale);
public Matrix4x4 Matrix4x4CreateFromTranslation(Vector3 translation);
public Matrix4x4 Matrix4x4CreateFromAxisAngle(Vector3 axis, float angle);
public Matrix4x4 Scale(Matrix4x4 value, float factor);
```
### 21.2.7. Quaternion Functions

```C#
public Quaternion Concatenate(Quaternion value, Quaternion value2);
public Quaternion CreateFromAxisAngle(Vector3 axis, float angle);
public Quaternion Normalize(Quaternion value);
public Quaternion Quaternion(float x, float y, float z, float w);
public Quaternion Slerp(Quaternion value1, Quaternion value2, float progress);
```
### 21.2.8. Color Functions

```C#
public Color ColorLerp(Color colorTo, Color colorFrom, float progression);
public Color ColorLerpHSL(Color colorTo, Color colorFrom, float progression);
public Color ColorLerpRGB(Color colorTo, Color colorFrom, float progression);
public Color ColorHsl(float h, float s, float l);
public Color ColorRgb(byte a, byte r, byte g, byte b);
```
### 21.2.9. Expression Templates - Targets

```C#
public AmbientLightTarget AmbientLightTarget();
public ColorBrushTarget ColorBrushTarget();
public DistantLightTarget DistantLightTarget();
public DropShadowTarget DropShadowTarget();
public InsetClipTarget InsetClipTarget();
public InteractionTrackerTarget InteractionTrackerTarget();
public ManipulationPropertySetTarget ManipulationPropertySetTarget();
public NineGridBrushTarget NineGridBrushTarget();
public PointerPositionPropertySetTarget PointerPositionPropertySetTarget();
public PointLightTarget PointLightTarget();
public SpotLightTarget SpotLightTarget();
public SurfaceBrushTarget SurfaceBrushTarget();
public VisualTarget VisualTarget();
```
### 21.2.10. Expression Templates - References

```C#
public AmbientLightReference AmbientLightReference(string referenceName);
public ColorBrushReference ColorBrushReference(string referenceName);
public DistantLightReference DistantLightReference(string referenceName);
public DropShadowReference DropShadowReference(string referenceName);
public InsetClipReference InsetClipReference(string referenceName);
public InteractionTrackerReference InteractionTrackerReference(string referenceName);
public ManipulationPropertySetReference ManipulationPropertySetReference(string referenceName);
public NineGridBrushReference NineGridBrushReference(string referenceName);
public PointerPositionPropertySetReference PointerPositionPropertySetReference(string referenceName);
public PointLightReference PointLightReference(string referenceName);
public SpotLightReference SpotLightReference(string referenceName);
public SurfaceBrushReference SurfaceBrushReference(string referenceName);
public VisualReference VisualReference(string referenceName);
```
