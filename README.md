# VideoViewer

使用[iyzyi/PornhubCreeper](https://github.com/iyzyi/PornhubCreeper)和[iyzyi/XvideosCreeper](https://github.com/iyzyi/XvideosCreeper)下载相应视频后，可以通过本项目方便地管理和播放。

只通过文件夹的标题，谁知道里面有啥内容啊，点进去还要点出来，好麻烦的。这个程序可以一次性展示很多个视频的标题和封面。

## 配置

在`config.ini`中更改相关配置，其中Type必须为`pornhub`或`xvideos`。

```
[Label]		pornhub-label
[Type]		pornhub
[DirPath]	D:\爬虫\pornhub

[Label]		xvideos-label
[Type]		xvideos
[DirPath]	D:\爬虫\xvideos
```

可以根据需要追加多组`config`。

## 使用

上下键切换分组，左右键切换当前分组的页数。