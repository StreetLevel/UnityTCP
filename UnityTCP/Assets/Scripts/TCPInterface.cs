
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class TCPInterface : MonoBehaviour
{

	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private TcpClient connectedTcpClient;
	private Dictionary<string,Mesh> meshdict;
	private Dictionary<string,GameObject> godict;
	private Queue<UnityMesh> meshqueue;
    private Queue<UnityCameraSettings> camsetqueue;
	private static string id_tri = " Surface";
	private static string id_line = " Line";
	private static string id_vert = " Point";
    public Shader shader_transparent;
    public Shader shader_smooth;
    public Shader shader_flat;
    public Shader shader_flat_wireframe;
    public Shader shader_point;
    public Shader shader_line;
    public Dictionary<string, Shader> shaders;
    private bool flag_reset = false;
	 private string clearnum = "all";
    private bool all_visible = true;
    private bool flag_take_screenshot = false;
    private String screenshot_filename = "";
    private int screenshot_frame = 0;
    private GameObject canvas;
	public int targetFrameRate = 30;
	public string listenIP = "127.0.0.1";
	public int listenPort = 8052;
	private string _listenIP = "127.0.0.1";
	private int _listenPort = 8052;
	private GameObject cam;
	private ModelCamera modelCamera;
	public bool serverListening = true;
	private bool _serverListening = true;

		// Use this for initialization
		void Start ()
		{
                GameObject button_clear = GameObject.Find("Button_clear");
                button_clear.GetComponent<Button>().onClick.AddListener(reset_all);

                GameObject button_toggle = GameObject.Find("Button_toggle");
                button_toggle.GetComponent<Button>().onClick.AddListener(toggle_all);

                canvas = GameObject.Find("Canvas");

				meshdict = new Dictionary<string,Mesh> ();
				godict = new Dictionary<string,GameObject> ();
				meshqueue = new Queue<UnityMesh> ();
                camsetqueue = new Queue<UnityCameraSettings>();

        shaders = new Dictionary<string, Shader>();
        shaders["transparent"] = shader_transparent;
        shaders["smooth"] = shader_smooth;
        shaders["flat"] = shader_flat;
        shaders["flat_wireframe"] = shader_flat_wireframe;
        shaders["point"] = shader_point;
        shaders["line"] = shader_line;

				// Start Server
				tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));
				tcpListenerThread.IsBackground = true;
				tcpListenerThread.Start();

				cam = GameObject.Find("Main Camera");
				modelCamera = cam.GetComponent<ModelCamera>();

		}


    public void reset_all ()
    {
        foreach(KeyValuePair<string,GameObject> entry in godict)
        {
			  	Debug.Log(entry.Key);
            Destroy(entry.Value);
        }
        godict.Clear();
        meshdict.Clear();
    }

	 public void reset(int id)
    {
		 	var remkeys = new List<String>();
        foreach(KeyValuePair<string,GameObject> entry in godict)
        {
			  var myid = int.Parse(entry.Key.Substring(0,1));
			if (myid == id) {
				remkeys.Add(entry.Key);
			}
        }
		  foreach(String remkey in remkeys) {
			  Destroy(godict[remkey]);
			  godict.Remove(remkey);
		  }
		  remkeys.Clear();
		  foreach(KeyValuePair<string,Mesh> entry in meshdict)
		  {
			 var myid = int.Parse(entry.Key.Substring(0,1));
		  if (myid == id) {
			  remkeys.Add(entry.Key);
		  }
		  }
		 foreach(String remkey in remkeys) {
			 Destroy(meshdict[remkey]);
			 meshdict.Remove(remkey);
		 }
    }

    public void toggle_all ()
    {
    	all_visible = !all_visible;
        foreach(KeyValuePair<string,GameObject> entry in godict)
        {
         if (entry.Value.GetComponent<Button>())
         {

         	ColorBlock thecolor = entry.Value.GetComponent<Button>().colors;

         	if ( (thecolor.normalColor == Color.red && all_visible) || (thecolor.normalColor == Color.green && !all_visible) )
         	{
         		entry.Value.GetComponent<Button>().onClick.Invoke();
         	}

         }
        }
    }

		// Update is called once per frame
		void Update ()
    {
		if (listenIP != _listenIP || listenPort != _listenPort) {
			try {
				IPAddress.Parse(listenIP);
				_listenPort = listenPort;
				_listenIP = listenIP;
			}
			catch (FormatException e) {
				Debug.LogWarning("Invalid IP address entered.");
				Debug.LogWarning(e.ToString());
			}
		}

		if (serverListening != _serverListening) {
			if (!serverListening) {
				Debug.Log("Server stopping.");
				tcpListener.Stop();
			}
			else {
				tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));
				tcpListenerThread.IsBackground = true;
				tcpListenerThread.Start();
			}
			_serverListening = serverListening;
		}
    	 QualitySettings.vSyncCount = 0;
         Application.targetFrameRate = targetFrameRate;

        while (camsetqueue.Count > 0)
            {
            UnityCameraSettings rec_set = camsetqueue.Dequeue();
            rec_set.process_command(cam,modelCamera);
            }

				while (meshqueue.Count > 0) {

					UnityMesh rec_msh = meshqueue.Dequeue();
					GameObject ago = null;
					GameObject parent = null;
					string[] rec_ids = rec_msh.id.Split(":"[0]);
					string rec_msh_id = rec_ids[0];
					string id_spec = "";
					if (rec_ids.Length>1){
						id_spec = rec_ids[1];
					}

					if (godict.ContainsKey (rec_msh_id))
					{
						parent = godict[rec_msh_id];
					}
					else
					{

						parent = new GameObject();
        				parent.name = rec_msh_id;
						godict[rec_msh_id] = parent;

                        //GameObject button = (GameObject)Instantiate(SampleButton);
                        GameObject prefab = GameObject.Find("Button_clear");
                        GameObject button = (GameObject)Instantiate(prefab);
                        GameObject panel = GameObject.Find("Panel_visible");
                        button.transform.SetParent(panel.transform);//Setting button parent
                                                                    //button.SetActive(true);
                        button.GetComponent<Button>().onClick.RemoveAllListeners();

                        //button.GetComponent<Button>().onClick.AddListener(OnClick);//Setting what button does when clicked
                        button.GetComponent<Button>().onClick.AddListener(() => on_button_click(parent,button) );
                        button.transform.GetChild(0).GetComponent<Text>().text = rec_msh_id;//Changing text

                        Button thebutton = button.GetComponent<Button>();
                        ColorBlock thecolor = button.GetComponent<Button>().colors;
                        thecolor.normalColor = Color.green;
                        thecolor.highlightedColor = Color.green;
                        thecolor.pressedColor = Color.green;
                        thebutton.colors = thecolor;

                        godict[rec_msh_id + ":Button"] = button;

					}

					if (rec_msh.triangles.Length > 2) {
							//triangle mesh
							string id_tri_msh = rec_msh_id + id_tri + " " + id_spec;

							if (meshdict.ContainsKey (id_tri_msh) && godict.ContainsKey (id_tri_msh)) {

									Mesh msh = meshdict [id_tri_msh];
									rec_msh.update_tri_mesh (msh);
									ago = godict [id_tri_msh];

							} else {

									ago = new GameObject (id_tri_msh);
									//ago.transform.SetParent(parent.transform);
									godict [id_tri_msh] = ago;
									Mesh msh = rec_msh.new_tri_mesh (ago,shaders["smooth"]);
									meshdict [id_tri_msh] = msh;
									//EditorGUIUtility.PingObject(ago);
									//Selection.activeGameObject = ago;
									//ago.transform.SetParent(parent.transform, false);

									ago.transform.parent = parent.transform;


							}
							rec_msh.process_options(godict [id_tri_msh], shaders, "surface");
					}
					if (rec_msh.lines.Length > 1) {
							//line mesh
							//string id_tri_msh = rec_msh_id + id_tri + " " + id_spec;
							string id_line_msh = rec_msh_id + " " + id_spec + " "+ id_line;
							if (meshdict.ContainsKey (id_line_msh) && godict.ContainsKey (id_line_msh)) {

									//update mesh
									Mesh msh = meshdict [id_line_msh];
									rec_msh.update_line_mesh (msh);
									ago = godict [id_line_msh]; //?

							} else {

									//new mesh
									ago = new GameObject (id_line_msh);
									//ago.transform.SetParent(parent.transform);
									//ago.transform.parent = parent.transform;
									godict [id_line_msh] = ago;
                    Mesh msh = rec_msh.new_line_mesh (ago,shaders["line"]);
									meshdict [id_line_msh] = msh;
									ago.transform.parent = parent.transform;


							}
							rec_msh.process_options(godict [id_line_msh], shaders,  "line");
					}
					if (rec_msh.points.Length > 0) {
							//vertex mesh
							string id_vert_msh = rec_msh_id + id_vert + " " + id_spec;
							if (meshdict.ContainsKey (id_vert_msh) && godict.ContainsKey (id_vert_msh)) {

									//update mesh
									Mesh msh = meshdict [id_vert_msh];
									rec_msh.update_vert_mesh (msh);
									ago = godict [id_vert_msh];

							} else {

									//new mesh
									ago = new GameObject (id_vert_msh);
									godict [id_vert_msh] = ago;
									//ago.transform.SetParent(parent.transform);
                    Mesh msh = rec_msh.new_vert_mesh (ago,shaders["point"]);
									meshdict [id_vert_msh] = msh;
									ago.transform.parent = parent.transform;
							}
							rec_msh.process_options(godict [id_vert_msh],shaders, "point");
					}
					if (ago!=null){
                        var children = new List<GameObject>();
                        foreach (Transform child in ago.transform) children.Add(child.gameObject);
                        children.ForEach(child => Destroy(child));
						rec_msh.draw_text(ago);
					}
					ago = null;
					rec_msh = null;

				}

				if (flag_reset)
				{
					flag_reset = false;
					if (clearnum == "all") {
						reset_all ();
					}
					else {
						try{
							reset(int.Parse(clearnum));
						}
						catch (Exception e) {
							Debug.LogWarning("Resetting failed.");
							Debug.Log(e.ToString());
							clearnum = "all";
						}
					}
				}

				if (flag_take_screenshot)
				{

					if (screenshot_frame==0)
					{

						canvas.SetActive(false);
						screenshot_frame++;
					}
					else
					{
						if (screenshot_frame==1)
						{
							ScreenCapture.CaptureScreenshot (screenshot_filename,2);
							screenshot_frame++;
						}
						else
						{
							if(screenshot_frame==2)
							{
								canvas.SetActive(true);
								Debug.Log("Screenshot taken");
                    			Debug.Log(screenshot_filename);
								flag_take_screenshot = false;
								screenshot_filename = "";
								screenshot_frame=0;
							}
						}
					}
				}

		}

		private void ListenForIncommingRequests () {
		try {
			// Create listener on localhost port 8052.
			//tcpListener = new TcpListener(IPAddress.Parse("130.75.53.247"), 5666);
			//tcpListener = new TcpListener(IPAddress.Parse("130.75.53.91"), 5666);
			//tcpListener = new TcpListener(IPAddress.Parse("130.75.53.250"), 5666);
			tcpListener = new TcpListener(IPAddress.Parse(_listenIP), _listenPort);
			tcpListener.Start();
			Debug.Log("Server is listening on " + _listenIP + ":" + _listenPort.ToString());
			//Byte[] bytes = new Byte[1024];
			Byte[] bytes = new Byte[96000];
			StringBuilder sb = new StringBuilder ();
			while (true) {
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) {
					// Get a stream object for reading
					using (NetworkStream stream = connectedTcpClient.GetStream()) {
						int length;
						// Read incomming stream into byte arrary.
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);
							// Convert byte array to string message.
							string clientMessage = Encoding.ASCII.GetString(incommingData);
                            //Debug.Log("client message received as: " + clientMessage);
                            if (clientMessage.Length > 24 && clientMessage.Substring(clientMessage.Length - 25, 25).Equals("UNITY_MESH_JSON_FORMATTED"))
                            {
                                //Debug.Log ("End of Message");
                                sb.Append(clientMessage.Substring(0, clientMessage.Length - 25));
                                UnityMesh rec_msh = JsonUtility.FromJson<UnityMesh>(sb.ToString());
                                meshqueue.Enqueue(rec_msh);
                                sb = new StringBuilder();
                            }
                            else
                            {
                                if (clientMessage.Length > 20 && clientMessage.Substring(clientMessage.Length - 21, 21).Equals("UNITY_CAMERA_SETTINGS"))
                                {
                                    sb.Append(clientMessage.Substring(0, clientMessage.Length - 21));
                                    UnityCameraSettings cam_settings = JsonUtility.FromJson<UnityCameraSettings>(sb.ToString());
                                    camsetqueue.Enqueue(cam_settings);
                                    sb = new StringBuilder();
                                }
                                else
                                {
                                	if (clientMessage.Length > 14 && clientMessage.Substring(clientMessage.Length - 15, 15).Equals("UNITY_RESET_ALL"))
                                	{
                                		flag_reset = true;
                                		sb = new StringBuilder();
                                	}
											else if (clientMessage.Length > 12 && clientMessage.Substring(clientMessage.Length - 13, 12).Equals("UNITY_RESET_"))
                                	{
												clearnum = clientMessage.Substring(clientMessage.Length - 1, 1);
                                		flag_reset = true;
                                		sb = new StringBuilder();
                                	}
                                	else
                                	{
                                		if (clientMessage.Length > 15 && clientMessage.Substring(clientMessage.Length - 16, 16).Equals("UNITY_SCREENSHOT"))
                                		{
                                			sb.Append(clientMessage.Substring(0, clientMessage.Length - 16));
                                			screenshot_filename = sb.ToString();
                                			flag_take_screenshot = true;
                                			sb = new StringBuilder();
                                		}
                                		else{
                                    		sb.Append(clientMessage);
                                    	}
                                	}
                                }
                            }
						}
					}
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
		private void SendMessage() {
		if (connectedTcpClient == null) {
			return;
		}

		try {
			// Get a stream object for writing.
			NetworkStream stream = connectedTcpClient.GetStream();
			if (stream.CanWrite) {
				string serverMessage = "This is a message from your server.";
				// Convert string message to byte array.
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
				// Write byte array to socketConnection stream.
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				Debug.Log("Server sent his message - should be received by client");
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}


    public void on_button_click(GameObject o, GameObject b)
    {
        if (o.activeSelf)
        {
            o.SetActive(false);
            Button thebutton = b.GetComponent<Button>();
            ColorBlock thecolor = b.GetComponent<Button>().colors;
            thecolor.normalColor = Color.red;
            thecolor.highlightedColor = Color.red;
            thecolor.pressedColor = Color.red;
            thebutton.colors = thecolor;


        }
        else
        {
            o.SetActive(true);
            Button thebutton = b.GetComponent<Button>();
            ColorBlock thecolor = b.GetComponent<Button>().colors;
            thecolor.normalColor = Color.green;
            thecolor.highlightedColor = Color.green;
            thecolor.pressedColor = Color.green;
            thebutton.colors = thecolor;

        }
    }

}
