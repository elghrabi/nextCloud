# nextCloud 
nextCloud is an android application sample for files storage. It is written in C# (Xamarin) and uses Parse cloud to store data and manage identity log-ins.

![nextcloud](https://user-images.githubusercontent.com/6306036/34407098-9138ed50-ebb4-11e7-8f72-71cfc6342826.jpg)

Shared code is in "SharedProj" folder and contains necessary classes
*   Data (Donnee.cs)
*   File (Fichier.cs).
*   NavDrawerItem.cs

Note that it is important to set the keys gotten in Parse.com (Application ID & .NET Key) on your code "**App.cs**"

```csharp
public override void OnCreate ()
{
  base.OnCreate ();

  // Initialize the parse client with your Application ID and .NET Key found on
  // your Parse dashboard
  ParseClient.Initialize("Application ID", ".NET Key");
}
```
## Layouts

#### Main Screen (No items)

![layout1](https://user-images.githubusercontent.com/6306036/34407645-5c3ae3ee-ebb7-11e7-91c8-9f020928658c.jpg)

#### Menu to add an item

![layout2](https://user-images.githubusercontent.com/6306036/34408004-3f0c4d42-ebb9-11e7-9189-46586b73c869.jpg)

#### Added items

![layout3](https://user-images.githubusercontent.com/6306036/34408049-741ada9e-ebb9-11e7-9af4-23b59cb53a24.jpg)

#### Sidebar Navigation

![layout4](https://user-images.githubusercontent.com/6306036/34408072-9104b49a-ebb9-11e7-9897-5d01af6aacb6.jpg)


# Author
Soufiane ELGHRABI
