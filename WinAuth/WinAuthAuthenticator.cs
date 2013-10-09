﻿/*
 * Copyright (C) 2013 Colin Mackie.
 * This software is distributed under the terms of the GNU General Public License.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Windows;
using System.Windows.Forms;

using WinAuth.Resources;

namespace WinAuth
{
	public interface IWinAuthAuthenticatorChangedListener
	{
		void OnWinAuthAuthenticatorChanged(WinAuthAuthenticator sender, WinAuthAuthenticatorChangedEventArgs e);
	}

	/// <summary>
	/// Wrapper for real authenticator data used to save to file with other application information
	/// </summary>
  public class WinAuthAuthenticator : ICloneable
  {
    /// <summary>
    /// Event handler fired when property is changed
    /// </summary>
		public event WinAuthAuthenticatorChangedHandler OnWinAuthAuthenticatorChanged;

		/// <summary>
		/// Unique Id of authenticator saved in config
		/// </summary>
    public Guid Id { get; set; }

		/// <summary>
		/// Index for authenticator when in sorted list
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Actual authenticator data
		/// </summary>
    public Authenticator AuthenticatorData { get; set; }

		/// <summary>
		/// When this authenticator was created
		/// </summary>
    public DateTime Created { get; set; }

		private string _name;
		private string _skin;
    private bool _autoRefresh;
    private bool _allowCopy;
    private bool _copyOnCode;
    private bool _hideSerial;
    private HotKey _hotkey;

		/// <summary>
		/// Create the authenticator wrapper
		/// </summary>
    public WinAuthAuthenticator()
    {
      Id = Guid.NewGuid();
      Created = DateTime.Now;
      _autoRefresh = true;
    }

		/// <summary>
		/// Clone this authenticator
		/// </summary>
		/// <returns></returns>
    public object Clone()
    {
      WinAuthAuthenticator clone = this.MemberwiseClone() as WinAuthAuthenticator;

      clone.Id = Guid.NewGuid();
			clone.OnWinAuthAuthenticatorChanged = null;
      clone.AuthenticatorData = (this.AuthenticatorData != null ? this.AuthenticatorData.Clone() as Authenticator : null);

      return clone;
    }

		/// <summary>
		/// Mark this authenticator as having changed
		/// </summary>
		public void MarkChanged()
		{
			if (OnWinAuthAuthenticatorChanged != null)
			{
				OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs());
			}
		}

		/// <summary>
		/// Get/set the name of this authenticator
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				if (OnWinAuthAuthenticatorChanged != null)
				{
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("Name"));
				}
			}
		}

		/// <summary>
		/// Set the skin for the authenticator (used for Icon)
		/// </summary>
		public string Skin
    {
      get
      {
        return _skin;
      }
      set
      {
        _skin = value;
        if (OnWinAuthAuthenticatorChanged != null)
        {
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("Skin"));
        }
      }
    }

    /// <summary>
    /// Get/set auto refresh flag
    /// </summary>
    public bool AutoRefresh
    {
      get
      {
        return _autoRefresh;
      }
      set
      {
        _autoRefresh = value;
				if (OnWinAuthAuthenticatorChanged != null)
        {
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("AutoRefresh"));
        }
      }
    }

    /// <summary>
    /// Get/set allow copy flag
    /// </summary>
    public bool AllowCopy
    {
      get
      {
        return _allowCopy;
      }
      set
      {
        _allowCopy = value;
				if (OnWinAuthAuthenticatorChanged != null)
        {
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("AllowCopy"));
        }
      }
    }

    /// <summary>
    /// Get/set auto copy flag
    /// </summary>
    public bool CopyOnCode
    {
      get
      {
        return _copyOnCode;
      }
      set
      {
        _copyOnCode = value;
				if (OnWinAuthAuthenticatorChanged != null)
        {
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("CopyOnCode"));
        }
      }
    }

    /// <summary>
    /// Get/set hide serial flag
    /// </summary>
    public bool HideSerial
    {
      get
      {
        return _hideSerial;
      }
      set
      {
        _hideSerial = value;
				if (OnWinAuthAuthenticatorChanged != null)
        {
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("HideSerial"));
        }
      }
    }

		/// <summary>
		/// Get/set the ketkey
		/// </summary>
		public HotKey HotKey
		{
			get
			{
				//if (this.AuthenticatorData != null && _hotkey != null)
				//{
				//	_hotkey.Advanced = this.AuthenticatorData.Script;
				//}
				return _hotkey;
			}
			set
			{
				_hotkey = value;
				//if (this.AuthenticatorData != null && _hotkey != null)
				//{
				//	AuthenticatorData.Script = _hotkey.Advanced;
				//}
				if (OnWinAuthAuthenticatorChanged != null)
				{
					OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs("HotKey"));
				}
			}
		}

		//public HoyKeySequence AutoLogin
		//{
		//	get
		//	{
		//		string script = this.AuthenticatorData.Script;
		//		if (string.IsNullOrEmpty(script) == true)
		//		{
		//			return null;
		//		}

		//		HoyKeySequence hks = new HoyKeySequence();
		//		using (XmlReader xr = XmlReader.Create(new StringReader(script)))
		//		{
		//			hks.ReadXml(xr);
		//		}
		//		return hks;
		//	}
		//	set
		//	{
		//		StringBuilder script = new StringBuilder();
		//		if (value != null)
		//		{
		//			using (XmlWriter xw = XmlWriter.Create(script))
		//			{
		//				value.WriteXmlString(xw);
		//			}
		//		}
		//		this.AuthenticatorData.Script = (script.Length != 0 ? script.ToString() : null);

		//		if (OnWinAuthAuthenticatorChanged != null)
		//		{
		//			OnWinAuthAuthenticatorChanged(this, new WinAuthAuthenticatorChangedEventArgs());
		//		}
		//	}
		//}

		public Bitmap Icon
		{
			get
			{
				if (string.IsNullOrEmpty(this.Skin) == false)
				{
					Stream stream;
					if (this.Skin.StartsWith("base64:") == true)
					{
						byte[] bytes = Convert.FromBase64String(this.Skin.Substring(7));
						stream = new MemoryStream(bytes, 0, bytes.Length);
					}
					else
					{
						stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WinAuth.Resources." + this.Skin);
					}
					if (stream != null)
					{
						return new Bitmap(stream);
					}
				}

				if (AuthenticatorData == null)
				{
					return null;
				}

				return new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("WinAuth.Resources." + AuthenticatorData.GetType().Name + "Icon.png"));
			}
			set
			{
				if (value == null)
				{
					this.Skin = null;
					return;
				}

				using (MemoryStream ms = new MemoryStream())
				{
					value.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					this.Skin = "base64:" + Convert.ToBase64String(ms.ToArray());
				}
			}
		}

		/// <summary>
		/// Copy the current code to the clipboard
		/// </summary>
		public void CopyCodeToClipboard(Form form, string code = null, bool showError = false)
		{
			if (code == null)
			{
				code = AuthenticatorData.CurrentCode;
			}

			bool clipRetry = false;
			do
			{
				bool failed = false;
				// check if the clipboard is locked
				if (WinAPI.GetOpenClipboardWindow() != IntPtr.Zero)
				{
					failed = true;
				}
				else
				{
					try
					{
						Clipboard.Clear();
						Clipboard.SetDataObject(code, true, 4, 250);
					}
					catch (ExternalException)
					{
						failed = true;
					}
				}

				if (failed == true && showError == true)
				{
					// only show an error the first time
					clipRetry = (MessageBox.Show(form, strings.ClipboardInUse,
						WinAuthMain.APPLICATION_NAME,
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
				}
			}
			while (clipRetry == true);
		}

    public void ReadXml(XmlReader reader, string password)
    {
      Guid id;
      if (Guid.TryParse(reader.GetAttribute("id"), out id) == true)
      {
        Id = id;
      }

      string authenticatorType = reader.GetAttribute("type");
      if (string.IsNullOrEmpty(authenticatorType) == false)
      {
        Type type = typeof(Authenticator).Assembly.GetType(authenticatorType, false, true);
        this.AuthenticatorData = Activator.CreateInstance(type) as Authenticator;
      }

			//string encrypted = reader.GetAttribute("encrypted");
			//if (string.IsNullOrEmpty(encrypted) == false)
			//{
			//	// read the encrypted text from the node
			//	string data = reader.ReadElementContentAsString();
			//	// decrypt
			//	Authenticator.PasswordTypes passwordType;
			//	data = Authenticator.DecryptSequence(data, encrypted, password, out passwordType);

			//	using (MemoryStream ms = new MemoryStream(Authenticator.StringToByteArray(data)))
			//	{
			//		reader = XmlReader.Create(ms);
			//		ReadXml(reader, password);
			//	}
			//	this.PasswordType = passwordType;
			//	this.Password = password;

			//	return;
			//}

      reader.MoveToContent();

      if (reader.IsEmptyElement)
      {
        reader.Read();
        return;
      }

      reader.Read();
      while (reader.EOF == false)
      {
        if (reader.IsStartElement())
        {
          switch (reader.Name)
          {
            case "name":
              Name = reader.ReadElementContentAsString();
              break;

            case "created":
              long t = reader.ReadElementContentAsLong();
              t += Convert.ToInt64(new TimeSpan(new DateTime(1970, 1, 1).Ticks).TotalMilliseconds);
              t *= TimeSpan.TicksPerMillisecond;
              Created = new DateTime(t).ToLocalTime();
              break;

            case "autorefresh":
              _autoRefresh = reader.ReadElementContentAsBoolean();
              break;

            case "allowcopy":
              _allowCopy= reader.ReadElementContentAsBoolean();
              break;

            case "copyoncode":
              _copyOnCode = reader.ReadElementContentAsBoolean();
              break;

            case "hideserial":
              _hideSerial = reader.ReadElementContentAsBoolean();
              break;

            case "skin":
              _skin = reader.ReadElementContentAsString();
              break;

						case "hotkey":
							_hotkey = new WinAuth.HotKey();
							_hotkey.ReadXml(reader);
							break;

						case "authenticatordata":
							try
							{
								// we don't pass the password as they are locked till clicked
								this.AuthenticatorData.ReadXml(reader);
							}
							catch (EncrpytedSecretDataException )
							{
								// no action needed
							}
							catch (BadPasswordException )
							{
								// no action needed
							}
              break;

						// v2
						case "authenticator":
							this.AuthenticatorData = Authenticator.ReadXmlv2(reader, password);
							break;
						// v2
						case "autologin":
              var hks = new HoyKeySequence();
              hks.ReadXml(reader, password);
              break;
						// v2
						case "servertimediff":
							this.AuthenticatorData.ServerTimeDiff = reader.ReadElementContentAsLong();
							break;


            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Read();
          break;
        }
      }
    }

    /// <summary>
    /// Write the data as xml into an XmlWriter
    /// </summary>
    /// <param name="writer">XmlWriter to write config</param>
    public void WriteXmlString(XmlWriter writer)
    {
      writer.WriteStartElement(typeof(WinAuthAuthenticator).Name);
      writer.WriteAttributeString("id", this.Id.ToString());
      if (this.AuthenticatorData != null)
      {
        writer.WriteAttributeString("type", this.AuthenticatorData.GetType().FullName);
      }

			//if (this.PasswordType != Authenticator.PasswordTypes.None)
			//{
			//	string data;

			//	using (MemoryStream ms = new MemoryStream())
			//	{
			//		XmlWriterSettings settings = new XmlWriterSettings();
			//		settings.Indent = true;
			//		settings.Encoding = Encoding.UTF8;
			//		using (XmlWriter encryptedwriter = XmlWriter.Create(ms, settings))
			//		{
			//			Authenticator.PasswordTypes savedpasswordType = PasswordType;
			//			PasswordType = Authenticator.PasswordTypes.None;
			//			WriteXmlString(encryptedwriter);
			//			PasswordType = savedpasswordType;
			//		}
			//		//data = Encoding.UTF8.GetString(ms.ToArray());
			//		data = Authenticator.ByteArrayToString(ms.ToArray());
			//	}

			//	string encryptedTypes;
			//	data = Authenticator.EncryptSequence(data, PasswordType, Password, out encryptedTypes);
			//	writer.WriteAttributeString("encrypted", encryptedTypes);
			//	writer.WriteString(data);
			//	writer.WriteEndElement();

			//	return;
			//}

      writer.WriteStartElement("name");
      writer.WriteValue(this.Name ?? string.Empty);
      writer.WriteEndElement();

      writer.WriteStartElement("created");
      writer.WriteValue(Convert.ToInt64((this.Created.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds));
      writer.WriteEndElement();

      writer.WriteStartElement("autorefresh");
      writer.WriteValue(this.AutoRefresh);
      writer.WriteEndElement();
      //
      writer.WriteStartElement("allowcopy");
      writer.WriteValue(this.AllowCopy);
      writer.WriteEndElement();
      //
      writer.WriteStartElement("copyoncode");
      writer.WriteValue(this.CopyOnCode);
      writer.WriteEndElement();
      //
      writer.WriteStartElement("hideserial");
      writer.WriteValue(this.HideSerial);
      writer.WriteEndElement();
      //
      writer.WriteStartElement("skin");
      writer.WriteValue(this.Skin ?? string.Empty);
      writer.WriteEndElement();
			//
			if (this.HotKey != null)
			{
				this.HotKey.WriteXmlString(writer);
			}

      // save the authenticator to the config file
      if (this.AuthenticatorData != null)
      {
        this.AuthenticatorData.WriteToWriter(writer);

        // save script with password and generated salt
				//if (this.AutoLogin != null)
				//{
				//	this.AutoLogin.WriteXmlString(writer, this.AuthenticatorData.PasswordType, this.AuthenticatorData.Password);
				//}
      }

      writer.WriteEndElement();
    }

  }

  /// <summary>
  /// Delegate for ConfigChange event
  /// </summary>
  /// <param name="source"></param>
  /// <param name="args"></param>
	public delegate void WinAuthAuthenticatorChangedHandler(WinAuthAuthenticator source, WinAuthAuthenticatorChangedEventArgs args);

  /// <summary>
  /// Change event arguments
  /// </summary>
  public class WinAuthAuthenticatorChangedEventArgs : EventArgs
  {
		public string Property { get; private set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public WinAuthAuthenticatorChangedEventArgs(string property = null)
    {
			Property = property;
    }

	}
}
