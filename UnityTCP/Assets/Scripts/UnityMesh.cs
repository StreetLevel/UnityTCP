using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class UnityMesh
{
	public string id;
	public Vector3[] vertices;
	public int[] points;
	public int[] lines;
	public int[] triangles;
	public Color[] colors;
	public string[] options;
	public UnityText[] text;
	byte[] pngBytes = new byte[] {
			0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A,0x00,0x00,0x00,0x0D,0x49,0x48,0x44,0x52,
			0x00,0x00,0x00,0x40,0x00,0x00,0x00,0x40,0x08,0x00,0x00,0x00,0x00,0x8F,0x02,0x2E,
			0x02,0x00,0x00,0x01,0x57,0x49,0x44,0x41,0x54,0x78,0x01,0xA5,0x57,0xD1,0xAD,0xC4,
			0x30,0x08,0x83,0x81,0x32,0x4A,0x66,0xC9,0x36,0x99,0x85,0x45,0xBC,0x4E,0x74,0xBD,
			0x8F,0x9E,0x5B,0xD4,0xE8,0xF1,0x6A,0x7F,0xDD,0x29,0xB2,0x55,0x0C,0x24,0x60,0xEB,
			0x0D,0x30,0xE7,0xF9,0xF3,0x85,0x40,0x74,0x3F,0xF0,0x52,0x00,0xC3,0x0F,0xBC,0x14,
			0xC0,0xF4,0x0B,0xF0,0x3F,0x01,0x44,0xF3,0x3B,0x3A,0x05,0x8A,0x41,0x67,0x14,0x05,
			0x18,0x74,0x06,0x4A,0x02,0xBE,0x47,0x54,0x04,0x86,0xEF,0xD1,0x0A,0x02,0xF0,0x84,
			0xD9,0x9D,0x28,0x08,0xDC,0x9C,0x1F,0x48,0x21,0xE1,0x4F,0x01,0xDC,0xC9,0x07,0xC2,
			0x2F,0x98,0x49,0x60,0xE7,0x60,0xC7,0xCE,0xD3,0x9D,0x00,0x22,0x02,0x07,0xFA,0x41,
			0x8E,0x27,0x4F,0x31,0x37,0x02,0xF9,0xC3,0xF1,0x7C,0xD2,0x16,0x2E,0xE7,0xB6,0xE5,
			0xB7,0x9D,0xA7,0xBF,0x50,0x06,0x05,0x4A,0x7C,0xD0,0x3B,0x4A,0x2D,0x2B,0xF3,0x97,
			0x93,0x35,0x77,0x02,0xB8,0x3A,0x9C,0x30,0x2F,0x81,0x83,0xD5,0x6C,0x55,0xFE,0xBA,
			0x7D,0x19,0x5B,0xDA,0xAA,0xFC,0xCE,0x0F,0xE0,0xBF,0x53,0xA0,0xC0,0x07,0x8D,0xFF,
			0x82,0x89,0xB4,0x1A,0x7F,0xE5,0xA3,0x5F,0x46,0xAC,0xC6,0x0F,0xBA,0x96,0x1C,0xB1,
			0x12,0x7F,0xE5,0x33,0x26,0xD2,0x4A,0xFC,0x41,0x07,0xB3,0x09,0x56,0xE1,0xE3,0xA1,
			0xB8,0xCE,0x3C,0x5A,0x81,0xBF,0xDA,0x43,0x73,0x75,0xA6,0x71,0xDB,0x7F,0x0F,0x29,
			0x24,0x82,0x95,0x08,0xAF,0x21,0xC9,0x9E,0xBD,0x50,0xE6,0x47,0x12,0x38,0xEF,0x03,
			0x78,0x11,0x2B,0x61,0xB4,0xA5,0x0B,0xE8,0x21,0xE8,0x26,0xEA,0x69,0xAC,0x17,0x12,
			0x0F,0x73,0x21,0x29,0xA5,0x2C,0x37,0x93,0xDE,0xCE,0xFA,0x85,0xA2,0x5F,0x69,0xFA,
			0xA5,0xAA,0x5F,0xEB,0xFA,0xC3,0xA2,0x3F,0x6D,0xFA,0xE3,0xAA,0x3F,0xEF,0xFA,0x80,
			0xA1,0x8F,0x38,0x04,0xE2,0x8B,0xD7,0x43,0x96,0x3E,0xE6,0xE9,0x83,0x26,0xE1,0xC2,
			0xA8,0x2B,0x0C,0xDB,0xC2,0xB8,0x2F,0x2C,0x1C,0xC2,0xCA,0x23,0x2D,0x5D,0xFA,0xDA,
			0xA7,0x2F,0x9E,0xFA,0xEA,0xAB,0x2F,0xDF,0xF2,0xFA,0xFF,0x01,0x1A,0x18,0x53,0x83,
			0xC1,0x4E,0x14,0x1B,0x00,0x00,0x00,0x00,0x49,0x45,0x4E,0x44,0xAE,0x42,0x60,0x82,
		};
    

	public UnityMesh(){
	}

	public Mesh new_tri_mesh(GameObject gameObject,Shader face_shader){

		Mesh msh = new Mesh();
		this.update_tri_mesh (msh);
		// Set up game object with mesh;
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		//filter.transform.parent = parent.transform;

		filter.mesh = msh;
        gameObject.GetComponent<Renderer>().material = new Material( face_shader );


		return msh;
	}
		
	public void update_tri_mesh(Mesh msh){
		bool facecolor = this.get_option("color") == "facecolor";
		if (!facecolor){
			msh.triangles = new int[]{};
			msh.vertices = this.vertices;
			msh.triangles = this.triangles;
			msh.colors = this.colors;
			msh.RecalculateNormals();
			msh.RecalculateBounds();
			msh.RecalculateTangents();
		}
		if(facecolor)
		{
			int len_tri = this.triangles.Length;
			int facenum = 0;

			Vector3[] new_vertices = new Vector3[len_tri];
			Color[] new_colors = new Color[len_tri];
			int[] new_triangles = new int[len_tri];

			for (int i = 0; i < len_tri; i++){
				facenum = i/3;
				new_vertices[i] = this.vertices[this.triangles[i]];
				new_colors[i] = this.colors[facenum];
				new_triangles[i] = i;
			}
			msh.vertices = new_vertices;
			msh.triangles = new_triangles;
			msh.colors = new_colors;
			msh.RecalculateNormals();
			msh.RecalculateBounds();
			msh.RecalculateTangents();
		}
	}

    public Mesh new_line_mesh(GameObject gameObject, Shader line_shader){
		//Debug.Log(this.id);
		Mesh msh = new Mesh();
		this.update_line_mesh (msh);
		// Set up game object with mesh;
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
        gameObject.GetComponent<Renderer>().material = new Material( line_shader );
		return msh;
	}

	public void update_line_mesh(Mesh msh){
		msh.vertices = this.vertices;
		msh.SetIndices(this.lines, MeshTopology.Lines, 0);
		msh.colors = this.colors;
		msh.RecalculateBounds();
	}

	public Mesh new_vert_mesh(GameObject gameObject, Shader vert_shader){
		Mesh msh = new Mesh();
		this.update_vert_mesh (msh);
		// Set up game object with mesh;
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = msh;
		//gameObject.GetComponent<Renderer>().material = new Material( Shader.Find("Point Cloud/Disk") );
        gameObject.GetComponent<Renderer>().material = new Material( vert_shader );
		return msh;
	}

	public void update_vert_mesh(Mesh msh){
		msh.vertices = this.vertices;
		msh.SetIndices(this.points, MeshTopology.Points, 0);
		msh.colors = this.colors;
		msh.RecalculateBounds();
	}

	public string get_option(string str){

		for(int i = 0; i < this.options.Length; i++){
			string[] strArr = this.options[i].Split("="[0]);
			if (strArr[0].Trim().Equals(str)){
				return strArr[1].Trim();
			}
		}
		return "not found";
	}

    public void process_options(GameObject gameObject, Dictionary<string, Shader> shaders, string str)
	{
		for(int i = 0; i < this.options.Length; i++)
		{	
			
			string[] strArr = this.options[i].Split("="[0]);
			if (strArr[0].Trim().Equals(str+"_shader") && str.Equals("surface")){
				//Debug.Log(strArr[1].Trim());
				set_surface_shader_options(gameObject,shaders,strArr[1].Trim());
			}
			if (strArr[0].Trim().Equals(str+"_size") && str.Equals("point") ){
				//Debug.Log(strArr[1].Trim());
				set_point_shader_size(gameObject, float.Parse( strArr[1].Trim() ) );
			}
			if (strArr[0].Trim().Equals(str+"_width") && str.Equals("line") ){
				//Debug.Log(strArr[1].Trim());
				set_line_shader_width(gameObject, float.Parse( strArr[1].Trim() ) );
			}

		}

	}

    public void set_surface_shader_options(GameObject gameObject, Dictionary<string, Shader> shaders, string option){
		if (option.Equals("wireframe")){
			//gameObject.GetComponent<Renderer>().material = new Material( Shader.Find("Custom/Flat Wireframe") );
            gameObject.GetComponent<Renderer>().material = new Material( shaders["flat_wireframe"] );
		}
		if (option.Equals("flat")){
            gameObject.GetComponent<Renderer>().material = new Material( shaders["flat"] );
		}
		if (option.Equals("smooth")){
            gameObject.GetComponent<Renderer>().material = new Material( shaders["smooth"] );
            Material material = gameObject.GetComponent<Renderer>().material;
            Texture2D tex = new Texture2D(2, 2);
        	tex.LoadImage(pngBytes);
        	material.SetTexture("_DetailAlbedoMap",tex);
        	material.SetFloat("_Metallic", 0.0f);
            SetupMaterialWithBlendMode(material, BlendMode.Opaque);
        	SetMaterialKeywords(material, WorkflowMode.Metallic);
        	
            //material.EnableKeyword("_DetailAlbedoMap");
		}
		if (option.Equals("transparent")){

            gameObject.GetComponent<Renderer>().material = new Material( shaders["transparent"] );
            Material material = gameObject.GetComponent<Renderer>().material;
            Texture2D tex = new Texture2D(2, 2);
        	tex.LoadImage(pngBytes);
        	material.SetTexture("_DetailAlbedoMap",tex);
        	material.SetFloat("_Metallic", 1.0f);
            SetupMaterialWithBlendMode(material, BlendMode.Transparent);
        	SetMaterialKeywords(material, WorkflowMode.Metallic);
        	
            //material.EnableKeyword("_DetailAlbedoMap");
		}

	}

	public void set_point_shader_size(GameObject gameObject, float size){
		gameObject.GetComponent<Renderer>().material.SetFloat("_PointSize", size);
	}

		public void set_line_shader_width(GameObject gameObject, float size){
		gameObject.GetComponent<Renderer>().material.SetFloat("_Width", size);
	}

	public void draw_text(GameObject parent){
		//foreach (Transform child in parent.transform) GameObject.Destroy(child.gameObject);
		for (int i = 0; i < this.text.Length; i++) draw_text(parent, this.text[i], i);
	}

	public void draw_text(GameObject parent, UnityText text, int i){
		GameObject textobject = new GameObject ("Text "+ i.ToString());
		TextMesh t = textobject.AddComponent<TextMesh>();
		t.text = text.text;
        t.color = text.color;
 		t.fontSize = 20;
 		t.transform.localEulerAngles = text.rot;
 		t.transform.position = text.pos;
 		t.transform.localScale = text.scale;
 		t.transform.parent = parent.transform;
	}

	private enum WorkflowMode
    {
        Specular,
        Metallic,
        Dielectric
    }

	public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }

    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }

    static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
    {
        int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
        if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
            return SmoothnessMapChannel.AlbedoAlpha;
        else
            return SmoothnessMapChannel.SpecularMetallicAlpha;
    }

        static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
    {
        // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
        // (MaterialProperty value might come from renderer material property block)
        SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
        if (workflowMode == WorkflowMode.Specular)
            SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
        else if (workflowMode == WorkflowMode.Metallic)
        SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
        SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
        SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

        // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
        // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
        // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
        MaterialEditor.FixupEmissiveFlag(material);
        bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
        SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

        if (material.HasProperty("_SmoothnessTextureChannel"))
        {
            SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
        }
        float intensity = material.GetFloat ( "_IntensityVC" );
        if ( intensity <= 0f )
        {
            SetKeyword ( material, "_VERTEXCOLOR_LERP", false );
            SetKeyword ( material, "_VERTEXCOLOR", false );
        }
        else if ( intensity > 0f && intensity < 1f )
        {
            SetKeyword ( material, "_VERTEXCOLOR_LERP", true );
            SetKeyword ( material, "_VERTEXCOLOR", false );
        }
        else
        {
            SetKeyword ( material, "_VERTEXCOLOR_LERP", false );
            SetKeyword ( material, "_VERTEXCOLOR", true );
        }
    }


	public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }


    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }

	static void MaterialChanged(Material material, WorkflowMode workflowMode)
    {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
        SetMaterialKeywords(material, workflowMode);
    }


	
}
