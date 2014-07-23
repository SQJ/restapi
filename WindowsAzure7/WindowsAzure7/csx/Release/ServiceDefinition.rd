<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="WindowsAzure7" generation="1" functional="0" release="0" Id="7d508158-4062-4c31-b136-5a22592fd5a1" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WindowsAzure7Group" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="MintRestApi:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/WindowsAzure7/WindowsAzure7Group/LB:MintRestApi:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="MintRestApi:DiagInfrastuctureLogLevel" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagInfrastuctureLogLevel" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:DiagLogsLogLevel" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagLogsLogLevel" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:DiagnosticMonitorEnabled" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagnosticMonitorEnabled" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:DiagPerfSampleRateSeconds" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagPerfSampleRateSeconds" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:DiagTransferPeriodMinutes" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagTransferPeriodMinutes" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:DiagWindowsEventLogLevel" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:DiagWindowsEventLogLevel" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="MintRestApi:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="MintRestApiInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WindowsAzure7/WindowsAzure7Group/MapMintRestApiInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:MintRestApi:Endpoint1">
          <toPorts>
            <inPortMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapMintRestApi:DiagInfrastuctureLogLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagInfrastuctureLogLevel" />
          </setting>
        </map>
        <map name="MapMintRestApi:DiagLogsLogLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagLogsLogLevel" />
          </setting>
        </map>
        <map name="MapMintRestApi:DiagnosticMonitorEnabled" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagnosticMonitorEnabled" />
          </setting>
        </map>
        <map name="MapMintRestApi:DiagPerfSampleRateSeconds" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagPerfSampleRateSeconds" />
          </setting>
        </map>
        <map name="MapMintRestApi:DiagTransferPeriodMinutes" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagTransferPeriodMinutes" />
          </setting>
        </map>
        <map name="MapMintRestApi:DiagWindowsEventLogLevel" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/DiagWindowsEventLogLevel" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapMintRestApi:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapMintRestApiInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApiInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="MintRestApi" generation="1" functional="0" release="0" software="D:\API\WindowsAzure7\WindowsAzure7\csx\Release\roles\MintRestApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DiagInfrastuctureLogLevel" defaultValue="" />
              <aCS name="DiagLogsLogLevel" defaultValue="" />
              <aCS name="DiagnosticMonitorEnabled" defaultValue="" />
              <aCS name="DiagPerfSampleRateSeconds" defaultValue="" />
              <aCS name="DiagTransferPeriodMinutes" defaultValue="" />
              <aCS name="DiagWindowsEventLogLevel" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;MintRestApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;MintRestApi&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="MintRestApi.svclog" defaultAmount="[1000,1000,1000]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApiInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApiUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApiFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="MintRestApiUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="MintRestApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="MintRestApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="4e20a8af-3406-4a8a-8e3b-053fe138f261" ref="Microsoft.RedDog.Contract\ServiceContract\WindowsAzure7Contract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="5adf84bb-2970-4711-955d-b8a59b7ed80d" ref="Microsoft.RedDog.Contract\Interface\MintRestApi:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/WindowsAzure7/WindowsAzure7Group/MintRestApi:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>