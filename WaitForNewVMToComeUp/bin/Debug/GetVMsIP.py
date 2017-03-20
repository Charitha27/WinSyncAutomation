

import XenAPI
import re
import sys,time

def read_ip_address(vm):
        vgm = session.xenapi.VM.get_guest_metrics(vm)
        try:
            os = session.xenapi.VM_guest_metrics.get_networks(vgm)
            if "0/ip" in os.keys():
                return os["0/ip"]
            return None
        except:
            return None

def read_os_name(vm):
        vgm = session.xenapi.VM.get_guest_metrics(vm)
        try:
            os = session.xenapi.VM_guest_metrics.get_os_version(vgm)
            if "name" in os.keys():
                return os["name"]
            return None
        except:
            return None
			
def main(session):
	
   f = open('C:\list.txt', 'w')

   vms = session.xenapi.VM.get_all_records()
   VMName="短测试输入字符㕝㐀䌫㒣㐁㐎㐏䶵䶴一丁丂丏䨩㸿㒣䶵㰦䬻㠯㘭长文集合舂赛博学北憧园畅微软辛盛爱家"
  # print ("VM name: %s" %VMName);
   for vm in vms:
		record = vms[vm]
		NameLabel=record["name_label"]
		uuid=record["uuid"]
		#print ("NameLabel name: %s" %NameLabel);
		#f.write(NameLabel);
		f.write(NameLabel.encode('utf8'))
		f.write("=")
		f.write(uuid)
		f.write("\n")

		#print "  Found  with name_label = %s" % (record["name_label"])
		#matchObj = re.match( "try", NameLabel)
		PatternObj=re.compile('%s'%VMName,re.UNICODE);
		matchObj = PatternObj.match(NameLabel)
		#if matchObj:
		if NameLabel.encode('utf8')==VMName:
			 print ("match found uuid: %s" %uuid)
			# print("Waiting for the VM %s to come up"%NameLabel);
			 #while read_os_name(vm) == None: time.sleep(1)
			 #print "Reported OS name: ", read_os_name(vm)
			 #Implementig time out. Will wait for a max of 40 mins for VM to come up.
			 timer=0
			 while read_ip_address(vm) == None and timer<=3600: 
				print("timer - %d"%timer)
				timer=timer+2
				time.sleep(2)
			 if timer>=3600:
				print "Time out waiting for VM\n";
			 else:
				print "Reported IP: ", read_ip_address(vm)
			 #return read_ip_address(vm)
			 
			
		
		
   
	
if __name__ == "__main__":
#def start():
    # if len(sys.argv) <> 5:
        # print "Usage:"
        # print sys.argv[0], " <url> <username> <password>"
        # sys.exit(1)
    url = sys.argv[1]
    username = sys.argv[2]
    password = sys.argv[3]
    # First acquire a valid session by logging in:
    session = XenAPI.Session(url)
    session.xenapi.login_with_password(username, password)
    try:
        main(session)
    except Exception, e:
        print str(e)
        raise


