# SileroVad
c# library for decoding silero vad Models，used in speech activity detection

##### 简介：

**SileroVad是一个使用C#编写的“语音端点检测”库，底层调用Microsoft.ML.OnnxRuntime对onnx模型进行解码，支持框架.Net6.0+，支持跨平台编译，支持AOT编译。使用简单方便。**

##### 支持的模型（ONNX）

| 模型名称  |  类型 | 下载地址  |
| ------------ | ------------ | ------------ |
|  silero-vad-v5-onnx | 端点检测  | [modelscope](https://modelscope.cn/models/manyeyes/silero-vad-v5-onnx "modelscope") |
|  silero-vad-onnx | 端点检测  | [modelscope](https://modelscope.cn/models/manyeyes/silero-vad-onnx "modelscope") |

##### 如何使用
###### 1.克隆项目源码
```bash
cd /path/to
git clone https://github.com/manyeyes/SileroVad.git
```
###### 2.下载上述列表中的模型到目录：/path/to/SileroVad/SileroVad.Examples
```bash
cd /path/to/SileroVad/SileroVad.Examples
git clone https://www.modelscope.cn/manyeyes/[模型名称].git
```
###### 3.使用vs2022(或其他IDE)加载工程，
###### 4.将模型目录中的文件设置为：复制到输出目录->如果较新则复制
###### 5.修改示例中代码：string modelName =[模型目录名]
程序入口点：Program.cs
默认执行以下两个测试用例
TestOfflineVad();
TestOnlineVad();
非流式示例源码：OfflineVad.cs
流式示例源码：OnlineVad.cs
源码中调用OfflineRecognizer方法进行语音识别，所以需要下载asr模型至项目中
```bash
cd /path/to/SileroVad/SileroVad.Examples
git clone https://www.modelscope.cn/manyeyes/aliparaformerasr-large-zh-en-timestamp-onnx-offline.git
```
###### 6.运行项目
###### 7.运行效果
```bash
// 非流式
load vad model elapsed_milliseconds:258.5859375
vad infer result:
00:00:00,002-->00:00:05,662
loading asr model elapsed_milliseconds:1251.890625
嗯 on time 要准时 in time 是及时交他总是准时交他的作业

00:00:05,986-->00:00:11,390
那用一般现在时是没有什么感情色彩的陈述一个事实

00:00:11,490-->00:00:15,454
下一句话为什么要用现在进行时他的意思并不是说

00:00:15,650-->00:00:17,640
说他现在正在教他的

elapsed_milliseconds:1856.7265625
total_duration:17640
rtf:0.10525660785147392
------------------------

// 流式
load vad model elapsed_milliseconds:75.21875
00:00:00,032-->00:00:05,632
嗯 on time 就要准时 in time 是及时交他总是准时交他的作业

------------------------------
00:00:06,016-->00:00:11,360
那用一般现在时是没有什么感情色彩的陈述一个事实

------------------------------
00:00:11,552-->00:00:15,424
下一句话为什么要用现在进行时他的意思并不是说

------------------------------
00:00:15,776-->00:00:21,696
说他现在正在教他的

------------------------------
elapsed_milliseconds:819.7734375
total_duration:17640
rtf:0.046472417091836735
------------------------
```

###### 相关工程：
* 语音识别，项目地址：[AliParaformerAsr](https://github.com/manyeyes/AliParaformerAsr "AliParaformerAsr") 
* 语音端点检测，解决长音频合理切分的问题，项目地址：[AliFsmnVad](https://github.com/manyeyes/AliFsmnVad "AliFsmnVad") 
* 文本标点预测，解决识别结果没有标点的问题，项目地址：[AliCTTransformerPunc](https://github.com/manyeyes/AliCTTransformerPunc "AliCTTransformerPunc")

###### 其他说明：

测试用例：AliParaformerAsr.Examples。
测试CPU：Intel(R) Core(TM) i7-10750H CPU @ 2.60GHz   2.59 GHz
支持平台：
Windows 7 SP1或更高版本,
macOS 10.13 (High Sierra) 或更高版本,ios等，
Linux 发行版（需要特定的依赖关系，详见.NET 6支持的Linux发行版列表），
Android（Android 5.0 (API 21) 或更高版本）。

引用参考
----------
[1] https://github.com/snakers4/silero-vad


