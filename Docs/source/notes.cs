           Logger logger = new Logger();
            ValidationResponse vr = new ValidationResponse();
            string dot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            logger.log("start", "Invoke-Bootstrap");


            // ══════════════════════════
            //  -Org Parameter
            // ══════════════════════════







            // ══════════════════════════
            //  -Org Parameter
            // ══════════════════════════

            if (!Uri.IsWellFormedUriString(org, UriKind.Absolute))
            {
                logger.log("error", "Uri is not well formatted. Check the provided Uri in your browser.");
                logger.log("fatal", "Invoke-Bootstrap");
                return;
            }

            try
            {
                ChefConfig.organizationUri = new Uri(org);
            }
            catch (Exception ex)
            {
                logger.log("error", "Org Parameter is invalid. Can not convert " + org + " to System.Uri. " + ex.Message + " Check the provided Uri in your browser.");
                logger.log("fatal", "Invoke-Bootstrap");
                return;
            }

            ChefConfig.organization = org.Substring(org.LastIndexOf("/") + 1, org.Length - org.LastIndexOf("/") - 1);
            if (ChefConfig.organization == "")
            {
                logger.log("error", "Organization name is empty or Uri contains trailing slashes. Remove trailing slashes from the Organization Uri.");
                logger.log("fatal", "Invoke-Bootstrap");
                return;
            }
            else
            {
                logger.log("ok", "Using Organizatoin name: " + ChefConfig.organization);
                Uri uri = new Uri(org);
                ChefConfig.organizationUri = uri;
                logger.log("ok", "Using Organizatoin Uri: " + ChefConfig.organizationUri.ToString());
            }

            // ══════════════════════════
            //  -Node Parameter
            // ══════════════════════════

            ChefConfig.nodeName = node;
            ChefConfig.clientName = node;

            // ══════════════════════════
            //  -KeyPath Parameter
            // ══════════════════════════

            if (keypath == null)
            {
                logger.log("info", "Key Parameter is not provided, switching to default .\\validation.pem");
                ChefConfig.clientPemPath = dot + @"\validation.pem";
                if (!File.Exists(dot + @"\validation.pem"))
                {
                    logger.log("error", "Key Parameter is not provided and default .\\validation.pem file can not be found.");
                    logger.log("fatal", "Invoke-Bootstrap");
                    return;
                }
            }
            else
            {
                if (!File.Exists(keypath))
                {
                    logger.log("info", keypath + "not found. Trying to find Key in the default path.");
                    ChefConfig.clientPemPath = keypath;
                }
                else if (!File.Exists(dot + "\\" + keypath))
                {
                    logger.log("ok", keypath + " found in de default path. Using: " + dot + "\\" + keypath);
                    ChefConfig.clientPemPath = dot + "\\" + keypath;
                }
                else
                {
                    logger.log("error", keypath + "not found.");
                    logger.log("fatal", "Invoke-Bootstrap");
                    return;
                }
            }

            // ══════════════════════════
            //  -Key Parameter
            // ══════════════════════════

            if (key != null)
            {
                ChefConfig.clientRsaKey = key;
            }



            // ══════════════════════════
            //  -Sudo Parameter
            // ══════════════════════════

            // TODO: Implement Sudo
            // Invoke-Bootstrap -Org https://betachef.schubergphilis.com/organizations/anoosh -Node Test_1


            // ══════════════════════════
            //  Check client.rb
            // ══════════════════════════

            ChefConfig.clientRbPath = dot + @"\client.rb";

            if (File.Exists(ChefConfig.clientRbPath))
            {
                if (force != null)
                {
                    File.Delete(ChefConfig.clientRbPath);
                    logger.log("warn", "client.rb file exists and -Force parameter is true. client.rb file is deleted.");
                }
                else
                {
                    logger.log("error", "client.rb file exists. Do not Invoke-Bootstrap if chef-client is already configured.");
                    logger.log("fatal", "Invoke-Bootstrap");
                    return;
                }
            }

            // ══════════════════════════
            //  Add Client
            // ══════════════════════════

            Client client = new Client();
            if (force != null)
            {
                vr = client.Add(ChefConfig.clientName, true);
            }
            else
            {
                vr = client.Add(ChefConfig.clientName, false);
            }

            if (vr.Result == 0)
            {
                using (StreamWriter sw = new StreamWriter(dot + @"\client.pem", false))
                {
                    sw.WriteLine(vr.Info);
                }
                logger.log("ok", "Chef client is succesfully created. RSA key is saved as client.pem");
            }
            else
            {
                logger.log("error", vr.Info);
                logger.log("fatal", "Invoke-Bootstrap");
                return;
            }


            // ══════════════════════════
            //  Create client.rb
            // ══════════════════════════

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("chef_server_url", "\"" + ChefConfig.organizationUri.ToString() + "\"");
            dict.Add("node_name", "\"" + ChefConfig.nodeName + "\"");

            try
            {
                StreamWriter sw = new StreamWriter(ChefConfig.clientRbPath);
                foreach (var param in dict)
                {
                    sw.WriteLine(String.Format("{0} {1}", param.Key, param.Value));
                }
                sw.Close();
                logger.log("ok", "client.rb file is succesfully created.");
            }
            catch (Exception ex)
            {
                logger.log("error", "Error writing client.rb file: " + ex.Message);
                logger.log("fatal", "Invoke-Bootstrap");
                return;
            }

            // ══════════════════════════
            //  Add Node
            // ══════════════════════════