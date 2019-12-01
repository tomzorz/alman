# alman

ALma (Apple) MANager is a .net core tool to manage your iPhone's camera output on Windows.

![](https://img.shields.io/badge/platform-windows-green.svg?longCache=true&style=flat-square) ![](https://img.shields.io/badge/nuget-yes-green.svg?longCache=true&style=flat-square) ![](https://img.shields.io/badge/license-MIT-blue.svg?longCache=true&style=flat-square)

You must have [.NET Core 3.0](https://www.microsoft.com/net/download/windows) or higher installed.

## Try the pre-built `alman`

You can quickly install and try [alman from nuget.org](https://www.nuget.org/packages/alman/) using the following commands:

```console
dotnet tool install -g alman
mkdir build
mkdir live
alman
```

> Note: You may need to open a new command/terminal window the first time you install a tool.

You can uninstall the tool using the following command:

```console
dotnet tool uninstall -g alman
```

## Features

### `alman sort`

Sort out a raw import of images/videos from an iPhone (in the current directory). This expects that you have `high efficiency` selected as your camera format settings.

This feature will try to sort contents in the following categories:

- saved images
- random images ("other")
- random videos ("other")
- screenshots
- raw images
- original images (the additional .jpg files will be renamed to match the originals)
- slow motion videos
- other

and "regular" pictures/vidos will be left as-is.

### `alman convert`

Takes the `.heic` images in the current folder and converts them to `.jpg` images. These will be placed in the `alman-convert` folder.

## Future

### Fix stuff

### Proper error handling

### ...