using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

using uPLibrary.Networking.M2Mqtt;  
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class MqttTest : MonoBehaviour {

	private MqttClient mqttClient;  

	public void SendMqtt () {
		// 加载证书
		var cert = Resources.Load ("cacert") as TextAsset;
		// 使用TLS证书连接
		mqttClient = new MqttClient("h5link.com", 3563, true, new X509Certificate(cert.bytes), new RemoteCertificateValidationCallback
			(
				// 测试服务器未设置公钥证书，返回true即跳过检查，直接通过，否则抛出服务器证书无效Error
				(srvPoint, certificate, chain, errors) => true
			));  
		// 消息接收事件
		mqttClient.MqttMsgPublishReceived += msgReceived;  
		// 连接
		mqttClient.Connect("Client ID", "admin", "password"); 
		// 订阅服务器返回消息
		mqttClient.Subscribe(new string[] { "Chat/HD_JoinChat" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });  
		// 发送登录消息
		mqttClient.Publish("Login/HD_Login/1", Encoding.UTF8.GetBytes("{\"userName\": \"username\",\"passWord\": \"Hello,anyone!\"}")); 
	}

	static void msgReceived(object sender, MqttMsgPublishEventArgs e)  
	{  
		Debug.Log("服务器返回数据");  
		string msg = System.Text.Encoding.Default.GetString(e.Message);  
		Debug.Log(msg);  
	}  

	public void OnDisable() {
		if (mqttClient != null && mqttClient.IsConnected)
			mqttClient.Disconnect ();
	}

}
