﻿// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader for Unity integration with SpriteLamp. Currently the 'kitchen sink'
// shader - contains all the effects from Sprite Lamp's preview window using the default shader.
// Based on a shader by Steve Karolewics & Indreams Studios. Final version by Finn Morgan
// Note: Finn is responsible for spelling 'colour' with a U throughout this shader. Find/replace if you must.


Shader "SpriteLamp/Standard_PerTexel"
{
    Properties
    {
        _MainTex ("Diffuse Texture", 2D) = "white" {}		//Alpha channel is plain old transparency
        _NormalDepth ("Normal Depth", 2D) = "bump" {} 		//Normal information in the colour channels, depth in the alpha channel.
        _SpecGloss ("Specular Gloss", 2D) = "" {}			//Specular colour in the colour channels, and glossiness in the alpha channel.
        _AmbientOcclusion ("Ambient Occlusion", 2D) = "" {} //A greyscale value for precomputed ambient occlusion - not very compact.
        _EmissiveColour ("Emissive colour", 2D) = "" {}		//A colour image that is simply added over the final colour. Might eventually have AO packed into its alpha channel.
       
        _SpecExponent ("Specular Exponent", Range (1.0,50.0)) = 10.0		//Multiplied by the alpha channel of the spec map to get the specular exponent.
        _SpecStrength ("Specular Strength", Range (0.0,5.0)) = 1.0		//Multiplier that affects the brightness of specular highlights
        _AmplifyDepth ("Amplify Depth", Range (0,1.0)) = 0.0	//Affects the 'severity' of the depth map - affects shadows (and shading to a lesser extent).
        _CelShadingLevels ("Cel Shading Levels", Float) = 0		//Set to zero to have no cel shading.
        _TextureRes("Texture Resolution", Vector) = (256, 256, 0, 0)	//Leave this to be set via a script.
        _AboveAmbientColour("Upper Ambient Colour", Color) = (0.3, 0.3, 0.3, 0.3)	//Ambient light coming from above.
        _BelowAmbientColour("Lower Ambient Colour", Color) = (0.1, 0.1, 0.1, 0.1)	//Ambient light coming from below.
        _LightWrap("Wraparound lighting", Range (0,1.0)) = 0.0	//Higher values of this will cause diffuse light to 'wrap around' and light the away-facing pixels a bit.
        _AmbientOcclusionStrength("Ambient Occlusion Strength", Range (0,1.0)) = 0.0	//Determines how strong the effect of the ambient occlusion map is.
        _EmissiveStrength("Emissive strength", Range(0, 1.0)) = 0.0	//Emissive map is multiplied by this.
        
        
        _SpotlightHardness("Spotlight hardness", Range(1.0, 10.0)) = 2.0	//Higher number makes the edge of a spotlight harder.
        _AttenuationExponent("Attenuation exponent", Range(0.0, 4.0)) = 2.0	//Higher number makes attenuation dropoff faster at first.
    }

    SubShader
    {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
        Pass
        {    
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 
            #pragma target 3.0
            #pragma glsl
            
			
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _NormalDepth;
            uniform sampler2D _SpecGloss;
            uniform sampler2D _AmbientOcclusion;
            uniform sampler2D _EmissiveColour;
            uniform float4 _AboveAmbientColour;
            uniform float4 _BelowAmbientColour;
            uniform float _AmbientOcclusionStrength;
            uniform float _EmissiveStrength;
            uniform float4 _LightColor0;
            uniform float _SpecExponent;
            uniform float _AmplifyDepth;
            uniform float _CelShadingLevels;
            uniform float4 _TextureRes;
            uniform float _LightWrap;
            uniform float _SpecStrength;
            uniform float4x4 unity_WorldToLight; // transformation
         	
           
            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 posLight : TEXCOORD2;
            };

            VertexOutput vert(VertexInput input) 
            {                
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.posWorld = mul(unity_ObjectToWorld, input.vertex);

                output.uv = input.uv.xy;
                output.color = input.color;
				output.posLight = mul(unity_WorldToLight, output.posWorld);
                return output;
            }

            float4 frag(VertexOutput input) : COLOR
            {
                float4 diffuseColour = tex2D(_MainTex, input.uv);
                float4 normalDepth = tex2D(_NormalDepth, input.uv);
                float ambientOcclusion = tex2D(_AmbientOcclusion, input.uv).r;
                float3 emissiveColour = tex2D(_EmissiveColour, input.uv).rgb;		
				float4 specGlossValues = tex2D(_SpecGloss, input.uv);
                
                float4 ambientResult;
                
                ambientOcclusion = (ambientOcclusion * _AmbientOcclusionStrength) + (1.0 - _AmbientOcclusionStrength);
        
                float3 worldNormalDirection = (normalDepth.xyz - 0.5) * 2.0;
                
                worldNormalDirection = float3(mul(float4(worldNormalDirection, 1.0), unity_WorldToObject).xyz);
                
                float upness = worldNormalDirection.y * 0.5 + 0.5; //'upness' - 1.0 means the normal is facing straight up, 0.5 means horizontal, 0.0 straight down, etc.
                
                float4 ambientColour = (_BelowAmbientColour * (1.0 - upness) + _AboveAmbientColour * upness) * ambientOcclusion;
                
                
                ambientResult = float4(ambientColour * diffuseColour + float4(emissiveColour * _EmissiveStrength, 0.0));
                
                //We have to calculate illumination here too, because the first light that gets rendered
                //gets folded into the ambient pass apparently.
                //Get the real vector for the normal, 
		        float3 normalDirection = (normalDepth.xyz - 0.5) * 2.0;
                normalDirection.z *= -1.0;
                normalDirection = normalize(normalDirection);
				
				
				
				
                float depthColour = normalDepth.a;
                
                float2 roundedUVs = input.uv;

                float3 vertexToLightSource;
                float3 lightDirection;
                
                if (0.0 == _WorldSpaceLightPos0.w) // directional light?
	            {
	            	//This handles directional lights
	              	lightDirection = normalize(float3(_WorldSpaceLightPos0.xyz));
	            }
                float aspectRatio = _TextureRes.x / _TextureRes.y;
                
                //We calculate shadows here. Magic numbers incoming (FIXME).
                float shadowMult = 1.0;
                float3 moveVec = lightDirection.xyz * 0.006 * float3(1.0, aspectRatio, -1.0);
                float thisHeight = depthColour * _AmplifyDepth;
               
                float3 tapPos = float3(roundedUVs, thisHeight + 0.1);
                //This loop traces along the light ray and checks if that ray is inside the depth map at each point.
                //If it is, darken that pixel a bit.
                for (int i = 0; i < 8; i++)
				{
					tapPos += moveVec;
					float tapDepth = tex2D(_NormalDepth, tapPos.xy).a * _AmplifyDepth;
					if (tapDepth > tapPos.z)
					{
						shadowMult -= 0.125;
					}
				}
                shadowMult = clamp(shadowMult, 0.0, 1.0);
                

                // Compute diffuse part of lighting
                float normalDotLight = dot(normalDirection, lightDirection);
                
                //Slightly awkward maths for light wrap.
                float diffuseLevel = clamp(normalDotLight + _LightWrap, 0.0, _LightWrap + 1.0) / (_LightWrap + 1.0) * shadowMult;
                
                // Compute specular part of lighting
                float specularLevel;
                if (normalDotLight < 0.0)
                {
                    // Light is on the wrong side, no specular reflection
                    specularLevel = 0.0;
                }
                else
                {
                    // For the moment, since this is 2D, we'll say the view vector is always (0, 0, -1).
                    //This isn't really true when you're not using a orthographic camera though. FIXME.
                    float3 viewDirection = float3(0.0, 0.0, -1.0);
                    specularLevel = pow(max(0.0, dot(reflect(-lightDirection, normalDirection),
                        viewDirection)), _SpecExponent * specGlossValues.a) * 0.4;
                }

                // Add cel-shading if enough levels were specified
                if (_CelShadingLevels >= 2.0)
                {
                    diffuseLevel = floor(diffuseLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5);
                    specularLevel = floor(specularLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5);
                }

				//The easy bits - assemble the final values based on light and map colours and combine.
                float3 diffuseReflection = diffuseColour.xyz * input.color.xyz * _LightColor0.xyz * diffuseLevel;
                float3 specularReflection = _LightColor0.xyz * input.color.xyz * specularLevel * specGlossValues.rgb * _SpecStrength;
                
                float4 finalColour = float4(diffuseReflection + specularReflection, diffuseColour.a) + ambientResult;
                
                return finalColour;
                

            }

            ENDCG
        }

        Pass
        {    
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One // additive blending 

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 
			#pragma target 3.0
			#pragma glsl
			
			#pragma multi_compile_lightpass
			

            #include "UnityCG.cginc"

            // User-specified properties
            uniform sampler2D _MainTex;
            uniform sampler2D _NormalDepth;
            uniform sampler2D _SpecGloss;
            uniform float4 _LightColor0;
            uniform float _SpecExponent;
            uniform float _AmplifyDepth;
            uniform float _CelShadingLevels;
            uniform float4 _TextureRes;
            uniform float _LightWrap;
            uniform float _SpecStrength;
            uniform float _SpotlightHardness;
            uniform float _AttenuationExponent;
            
            uniform float4x4 unity_WorldToLight; // transformation

            struct VertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 posLight : TEXCOORD2;
            };

            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.posWorld = mul(unity_ObjectToWorld, input.vertex);

                output.uv = input.uv.xy;
                output.color = input.color;
				output.posLight = mul(unity_WorldToLight, output.posWorld);
                return output;
            }

            float4 frag(VertexOutput input) : COLOR
            {
            	//Do texture reads first, because in theory that's a bit quicker...
                float4 diffuseColour = tex2D(_MainTex, input.uv);
				float4 normalDepth = tex2D(_NormalDepth, input.uv);				
				float4 specGlossValues = tex2D(_SpecGloss, input.uv);
				
				//Get the real vector for the normal, 
		        float3 normalDirection = (normalDepth.xyz - 0.5) * 2.0;
                normalDirection.z *= -1.0;
                normalDirection = normalize(normalDirection);
				
				
                float depthColour = normalDepth.a;
                
                
                float2 roundedUVs = input.uv;
                
                
#if (!(defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)))
                //For per-texel lighting, we recreate the world position based on the sprite's UVs...
                float2 positionOffset = input.uv;
                
                //Intervening here to round the UVs to the nearest 1.0/TextureRes to clamp the world position
                //to the nearest pixel...
                roundedUVs *= _TextureRes.xy;
                roundedUVs = floor(roundedUVs) + 0.5;
                roundedUVs /= _TextureRes.xy;
                
                
                //This is the per-texel stuff! It's a work in progress - that's why it's commented out right now.
                
                float2 uvCorrection = input.uv - roundedUVs;
                
                //Get tangent and bitangent stuff:
                float3 p_dx = ddx(input.posWorld.xyz);
				float3 p_dy = ddy(input.posWorld.xyz);
				//Rate of change of the texture coords
				float2 tc_dx = ddx(input.uv.xy);
				float2 tc_dy = ddy(input.uv.xy);
				//Initial tangent and bitangent
				
				float3 fragTangent = ( tc_dy.y * p_dx - tc_dx.y * p_dy ) / (length(p_dy) * length(p_dy));
				float3 fragBitangent = ( tc_dy.x * p_dx - tc_dx.x * p_dy ) / (length(p_dx) * length(p_dx));
				
				fragTangent /= (length(fragTangent) * length(fragTangent));
				fragBitangent /= (length(fragBitangent) * length(fragBitangent));
				
				//This corrects for nonuniform scales that reverse the handedness of the whole thing.
				float uvCross = clamp((cross(fragTangent, fragBitangent)).z * -10000000.0, -1.0, 1.0);
				
				float3 uVector = -normalize(fragTangent) * length(fragBitangent) * uvCross;
				float3 vVector = normalize(fragBitangent) * length(fragTangent) * uvCross;
                
                
                
                float3 posWorld = input.posWorld.xyz + uVector * -uvCorrection.x + vVector * -uvCorrection.y;
#else
				float3 posWorld = input.posWorld.xyz;
#endif

                posWorld.z -= depthColour * _AmplifyDepth;	//The fragment's Z position is modified based on the depth map value.
                float3 vertexToLightSource;
                float3 lightDirection;
                float attenuation;
                
#if (defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE))
            	//This handles directional lights
              	lightDirection = normalize(float3(_WorldSpaceLightPos0.xyz));
       	   		attenuation = 1.0;
#else
            	//This code is for point/spot lights. Note that light cookies aren't yet handled for spot lights yet (FIXME)
            	float cookieAttenuation = 1.0;
         	    float normalisedDistance = 1.0;
        		vertexToLightSource = float3(_WorldSpaceLightPos0.xyz) - posWorld;
            	
            	float lightDistance = length(vertexToLightSource);
            	
#if SPOT
            	//This number, 'distance from centre', is the distance this fragment is from the centre line
            	//of the spot light. If it is greater than 1.0, this fragment is outside the light cone and shouldn't be
            	//illuminated.
            	float distanceFromCentre = length (float2(input.posLight.xy) / input.posLight.w) * 2.0;
            	normalisedDistance = input.posLight.z;
            	
            	//Fairly simplistic implementation of a default spotlight shape. Not total rubbish like the last one was,
            	//and doesn't require a texture lookup, but still probably not perfect.
            	cookieAttenuation = (1.0 - distanceFromCentre) * _SpotlightHardness * _SpotlightHardness;
            	cookieAttenuation = clamp(cookieAttenuation, 0.0, 1.0);
            	
#elif POINT_NOATT
	            normalisedDistance = 0.0;
	            cookieAttenuation = 1.0;
#else
	    		cookieAttenuation = 1.0;
	    		normalisedDistance = length(input.posLight.xyz);
	    		//return float4(input.posLight.xyz, 0.0);
            	
#endif
	        	lightDirection = float3(mul(float4(vertexToLightSource, 1.0), unity_ObjectToWorld).xyz);
	        	lightDirection = normalize(lightDirection);
                
                attenuation = 1.0 - normalisedDistance;
                attenuation = clamp(attenuation, 0.0, 1.0);
                attenuation  = pow(attenuation, _AttenuationExponent);
                
                attenuation *= cookieAttenuation;
                
	            
#endif
                       
                float aspectRatio = _TextureRes.x / _TextureRes.y;
                
                //We calculate shadows here. Magic numbers incoming (FIXME).
                float shadowMult = 1.0;
                float3 moveVec = lightDirection.xyz * 0.006 * float3(1.0, aspectRatio, -1.0);
                float thisHeight = depthColour * _AmplifyDepth;
               
                float3 tapPos = float3(roundedUVs, thisHeight + 0.1);
                //This loop traces along the light ray and checks if that ray is inside the depth map at each point.
                //If it is, darken that pixel a bit.
                for (int i = 0; i < 8; i++)
				{
					tapPos += moveVec;
					float tapDepth = tex2D(_NormalDepth, tapPos.xy).a * _AmplifyDepth;
					if (tapDepth > tapPos.z)
					{
						shadowMult -= 0.125;
					}
				}
                shadowMult = clamp(shadowMult, 0.0, 1.0);

                // Compute diffuse part of lighting
                float normalDotLight = dot(normalDirection, lightDirection);
                
                //Slightly awkward maths for light wrap.
                float diffuseLevel = clamp(normalDotLight + _LightWrap, 0.0, _LightWrap + 1.0) / (_LightWrap + 1.0) * attenuation * shadowMult;
                
                // Compute specular part of lighting
                float specularLevel;
                if (normalDotLight < 0.0)
                {
                    // Light is on the wrong side, no specular reflection
                    specularLevel = 0.0;
                }
                else
                {
                    // For the moment, since this is 2D, we'll say the view vector is always (0, 0, -1).
                    //This isn't really true when you're not using a orthographic camera though. FIXME.
                    float3 viewDirection = float3(0.0, 0.0, -1.0);
                    specularLevel = attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection),
                        viewDirection)), _SpecExponent * specGlossValues.a) * 0.4;
                }

                // Add cel-shading if enough levels were specified
                if (_CelShadingLevels >= 2.0)
                {
                    diffuseLevel = floor(diffuseLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5);
                    specularLevel = floor(specularLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5);
                }
				//The easy bits - assemble the final values based on light and map colours and combine.
                float3 diffuseReflection = diffuseColour.xyz * input.color * _LightColor0.xyz * diffuseLevel;
                float3 specularReflection = _LightColor0.xyz * input.color * specularLevel * specGlossValues.rgb * _SpecStrength;
                
                float4 finalColour = float4((diffuseReflection + specularReflection) * diffuseColour.a, 0.0);
                //finalColour = input.posLight;//float4(normalisedDistance, normalisedDistance, normalisedDistance, 1.0);
                //finalColour = float4(length(input.posLight.xyz), 0.0, 0.0, 1.0);
                return finalColour;
                
             }

             ENDCG
        }
    }
    // The definition of a fallback shader should be commented out 
    // during development:
    Fallback "Transparent/Diffuse"
}