[pMixins]
======


###Welcome to [pMixins]!
http://pmixins.com
  
[pMixins] is a [Mixin](http://en.wikipedia.org/wiki/Mixins)
framework for C#.  Mixin code is generated at design time by a Visual Studio plug-in.  You can download the plug-in here: [https://visualstudiogallery.msdn.microsoft.com/a40fde9e-0e3f-4cdc-9c2a-af9de11695b2](https://visualstudiogallery.msdn.microsoft.com/a40fde9e-0e3f-4cdc-9c2a-af9de11695b2) 
  
Example usage:
  
**Step 1:** Define a Mixin Class that contains members that should be injected into other classes.
```csharp
public class Mixin
{
  // This method should be in several classes
  public void Method(){ }
}
```
  
**Step 2:** Define a Target class that should get all of `Mixin`'s Members (Note: This must be a `partial` class)
```csharp
[pMixin(Target = typeof(Mixin)]
public partial class Target{}
```
  
**Step 3:** Save Target.cs.  The `[pMixin] Code Generator` then gets to work generating the Composition code behind the scenes.  Below is a simplified version to illustrate what's going on.
```csharp
public partial class Target
{
  private Mixin _compositionInstance; 
  
  public void Method()
  {
    // forward call to Mixin instance
    _compositionInstance.Method();
  }
}
```
  
**Step 4:**  You can now create an instance of `Target` and call the mixed in `Mixin` members!
```csharp
public class Consumer  
{
  public void Example()
  {
    var target = new Target();
        
    // can call mixed in method
    target.Method();
        
    // can implicitly convert Target to Mixin
    Mixin m = new Target();
    m.Method();
  }
}
 
