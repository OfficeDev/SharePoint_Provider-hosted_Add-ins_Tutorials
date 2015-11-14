# A Series of Visual Studio Solutions to Accompany the MSDN Tutorial Series about Provider-hosted Add-ins #

This repository is a series of *provider-hosted* SharePoint Add-in samples that are used with the tutorial series that begins with [Get started creating provider-hosted SharePoint Add-ins](https://msdn.microsoft.com/EN-US/library/office/fp142381.aspx).

None of the samples in this repository contain *Sharepoint-hosted* SharePoint Add-ins. For information about the differences between the two types of SharePoint Add-ins, see [SharePoint Add-ins](http://msdn.microsoft.com/en-us/library/office/fp179930.aspx). 

### Applies to ###
-  SharePoint Online and on-premise SharePoint 2013 and later 

### Prerequisites ###
We recommend that you read the MSDN article [SharePoint Add-ins](http://msdn.microsoft.com/en-us/library/office/fp179930.aspx).

----------

# Overview #
This set of samples tracts the series of tutorials that begin at [Get started creating provider-hosted SharePoint Add-ins](https://msdn.microsoft.com/EN-US/library/office/fp142381.aspx). Each tutorial adds to the sample SharePoint add-in, **Chain Store**. This series of samples preserves the state of the add-in's Visual Studio solution after each tutorial. Before beginning any tutorial, you can open the corresponding sample solution in Visual Studio and follow along. For example, to follow the tutorial [Give the add-in the SharePoint look-and-feel](), open the BeforeSharePointUI.sln file in Visual Studio. 

The following are the tutorial articles and the corresponding sample solutions:

- [Get started creating provider-hosted SharePoint Add-ins](https://msdn.microsoft.com/EN-US/library/office/fp142381.aspx): N/A
- [Give the add-in the SharePoint look-and-feel](): BeforeSharePointUI.sln
- [Include a custom button in the add-in](): BeforeRibbonButton.sln
- [Get a quick overview of the SharePoint object model](): N/A
- [Add SharePoint write operations to the add-in](): BeforeSharePointWriteOps.sln
- [Include an add-in Web Part in the add-in](): BeforeAdd-inPart.sln
- [Handle add-in events in the add-in](): BeforeAdd-inEventHandlers.sln
- [Add first-run logic to the add-in](): BeforeFirstRunLogic.sln
- [Programmatically deploy a custom button in the add-in](): BeforeProgrammaticButton.sln
- [Handle list item events in the add-in](): BeforeRER.sln

To the the complete Chain Store add-in, open FinalChainStoreAdd-in.sln

# To use these samples #

12. Open **Visual Studio** as an administrator.
13. Open the .sln file for the solution you need.
13. In **Solution Explorer**, highlight the SharePoint add-in project and replace the **Site URL** property with the URL of your SharePoint developer site.
14. Follow the instructions in the corresponding tutorial to add functionality to the add-in and test it.

# Questions and comments

We'd love to get your feedback on this set of samples. You can send your questions and suggestions to us in the [Issues](https://github.com/OfficeDev/SharePoint_Provider-hosted_Add-ins_Tutorials/issues) section of this repository.
  
<a name="resources"/>
# Additional resources

* [SharePoint Add-ins](http://msdn.microsoft.com/en-us/library/office/fp179930.aspx)

### Copyright ###

Copyright (c) Microsoft. All rights reserved.




