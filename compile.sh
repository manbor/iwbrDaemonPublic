#!/bin/bash
echo "----------------------------------------------------------------------"
echo "			  stopping service"
echo "----------------------------------------------------------------------"
sudo systemctl stop iwbrDaemon 
sleep 2
systemctl -q status iwbrDaemon 


echo "----------------------------------------------------------------------"
echo "                     clean bin dir"
echo "----------------------------------------------------------------------"
rm -fv ~/iwbrDaemon/*


echo "----------------------------------------------------------------------"
echo "			fetch last git version"
echo "----------------------------------------------------------------------"
git reset --hard 
git fetch --all
git reset --hard
git pull 


echo "----------------------------------------------------------------------"
echo "				build"
echo "----------------------------------------------------------------------"
dotnet build ./iwbrDaemon/iwbrDaemon.csproj --output ~/iwbrDaemon/ /nowarn:*

if ! [ $? -eq 0 ]; then
    exit 1
fi


echo "----------------------------------------------------------------------"
echo "			deploy linux config"
echo "----------------------------------------------------------------------"
cp -v ./iwbrDaemon/_config_linux/* ~/iwbrDaemon/


echo "----------------------------------------------------------------------"
echo "   	        starting service"
echo "----------------------------------------------------------------------"
sudo systemctl start iwbrDaemon 
sleep 2
systemctl -q status iwbrDaemon 

echo "----------------------------------------------------------------------"
echo "	        	      show log"
echo "----------------------------------------------------------------------"
tail -f ~/iwbrDaemon/logs/iwbrDaemonAll.log
#tail -n 2000 -f ~/iwbrDaemon/logs/iwbrDaemonInfo.log
