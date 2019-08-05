# HostChange
HostChange is a console application to help manage different configurations for a windows hosts file.. 

### What do I need to run it?
HostChange is console application that requires a DotNet version 4.7.2+. Given that a system file is modified, **administration privileges** are required too.

### How do create sections?
To create a section, add a new line starting with **#CFG_START>**, followed by the section name, and optionally with a description enclosed by []. On the next lines, add the IPs entries and finish the section with a new line containing **#END**

Example
#CFG_START>MySectionName [My section Description]
xx.xx.xx.xx      url
xx.xx.xx.xx      url
xx.xx.xx.xx      url
#END

In this example a new section named **MySectionName** was created. This section has as description **My section Description** and 3 IPs.

So you can create several sections and toggle them using HostChange later. Ex.

```
#CFG_START>Config1 [Sample Configuration 1]
#103.20.70.200    demo.acme.co.xx
#103.20.70.201    off.acme.co.xx
#END
#CFG_START>Config2 [Sample Configuration 2]
#203.30.88.200    prod.acme.co.xx
#203.30.88.201    on.acme.co.xx
#END
#CFG_START>Config3 [Sample Configuration 3]
#211.97.12.200    test.acme.co.xx
#211.97.12.201    onoff.acme.co.xx
#END
```

### How do i run it?
Open command prompt, as Administrator, and execute HostChange.exe.

* HostChange.exe -list

will list all sections configured in your hosts.
```
#Config1 -> Sample Configuration 1
#Config2 -> Sample Configuration 2
#Config3 -> Sample Configuration 3
```

* HostChange.exe -fulllist

will list all hosts. contents
```
#CFG_START>Config1 [Sample Configuration 1]
#103.20.70.200    demo.acme.co.xx
#103.20.70.201    off.acme.co.xx
#END
#CFG_START>Config2 [Sample Configuration 2]
#203.30.88.200    prod.acme.co.xx
#203.30.88.201    on.acme.co.xx
#END
#CFG_START>Config3 [Sample Configuration 3]
#211.97.12.200    test.acme.co.xx
#211.97.12.201    onoff.acme.co.xx
#END
```

* HostChange.exe Config1

Will change the current configuration to **Config1**. You can switch configuration at any time. After HostChange apply the new configuration it will perform a flushdns.
