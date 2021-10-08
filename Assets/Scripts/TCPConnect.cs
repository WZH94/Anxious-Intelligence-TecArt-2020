using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPConnect : MonoBehaviour
{
	[SerializeField] public string IP;
	[SerializeField] private int port;
  [SerializeField] private DataProcessor dataProcessor;
  private bool frameUpdated = false;

	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread; 	
	#endregion  	
	// Use this for initialization 	
	void Start () {
    ConnectToTcpServer();
  }  	
	// Update is called once per frame
	void Update () {
    frameUpdated = true;
  }  	
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();
    } 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient("localhost", 8052);  			
			byte[] bytes = new byte[1024];             
			while (true)
      {
        // Get a stream object for reading 				
        using (NetworkStream stream = socketConnection.GetStream())
        {
          int length;
          // Read incomming stream into byte arrary. 					
          while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
          {
            if (frameUpdated)
            {
              frameUpdated = false;
              var incommingData = new byte[length];
              Array.Copy(bytes, 0, incommingData, 0, length);
              // Convert byte array to string message. 						
              string serverMessage = Encoding.ASCII.GetString(incommingData);
              //Debug.Log("server message received as: " + serverMessage);

              dataProcessor.ReceiveKinectData(serverMessage);
            }
          } // while
        } // using
			} // while (true)
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}
}