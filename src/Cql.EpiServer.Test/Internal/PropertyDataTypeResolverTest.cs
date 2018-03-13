﻿using System;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using FluentAssertions;
using Xunit;

namespace Cql.EpiServer.Test.Internal
{
    public class PropertyDataTypeResolverTest
    {
        [Theory]
        [InlineData(MetaDataProperties.PageLink, PropertyDataType.PageReference)]
        [InlineData(MetaDataProperties.PageTypeID, PropertyDataType.PageType)]
        [InlineData(MetaDataProperties.PageParentLink, PropertyDataType.PageReference)]
        [InlineData(MetaDataProperties.PagePendingPublish, PropertyDataType.Boolean)]
        [InlineData(MetaDataProperties.PageWorkStatus, PropertyDataType.Number)]
        [InlineData(MetaDataProperties.PageDeleted, PropertyDataType.Boolean)]
        [InlineData(MetaDataProperties.PageSaved, PropertyDataType.Boolean)]
        [InlineData(MetaDataProperties.PageTypeName, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageChanged, PropertyDataType.Date)]
        [InlineData(MetaDataProperties.PageCreatedBy, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageChangedBy, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageDeletedBy, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageDeletedDate, PropertyDataType.Date)]
        [InlineData(MetaDataProperties.PageCreated, PropertyDataType.Date)]
        [InlineData(MetaDataProperties.PageMasterLanguageBranch, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageLanguageBranch, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageGUID, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageContentAssetsID, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageContentOwnerID, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageVisibleInMenu, PropertyDataType.Boolean)]
        [InlineData(MetaDataProperties.PageURLSegment, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PagePeerOrder, PropertyDataType.Number)]
        [InlineData(MetaDataProperties.PageExternalURL, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageChangedOnPublish, PropertyDataType.Boolean)]
        [InlineData(MetaDataProperties.PageCategory, PropertyDataType.Category)]
        [InlineData(MetaDataProperties.PageStartPublish, PropertyDataType.Date)]
        [InlineData(MetaDataProperties.PageStopPublish, PropertyDataType.Date)]
        [InlineData(MetaDataProperties.PageArchiveLink, PropertyDataType.PageReference)]
        [InlineData(MetaDataProperties.PageShortcutType, PropertyDataType.Number)]
        [InlineData(MetaDataProperties.PageShortcutLink, PropertyDataType.PageReference)]
        [InlineData(MetaDataProperties.PageTargetFrame, PropertyDataType.Number)]
        [InlineData(MetaDataProperties.PageLinkURL, PropertyDataType.String)]
        [InlineData(MetaDataProperties.PageName, PropertyDataType.String)]
        public void Test_can_resolve_meta_data_property_type(string propertyName, PropertyDataType expectedResult)
        {
            // Arrange
            PropertyDataTypeResolver resolver = new PropertyDataTypeResolver(null);

            // Act
            bool successfullyResolvedProperty =
                resolver.TryResolve(propertyName, out PropertyDataType resolvedPropertyDataType);

            // Assert
            successfullyResolvedProperty.Should().BeTrue();
            resolvedPropertyDataType.ShouldBeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Test_can_resolve_property_type()
        {
            // Arrange
            const string propertyName = "TestProperty";
            const PropertyDataType propertyDataType = PropertyDataType.Boolean;

            ContentType contentType = new ContentType();
            contentType.PropertyDefinitions.Add(new PropertyDefinition
            {
                Name = propertyName,
                Type = new PropertyDefinitionType(1, propertyDataType, "Boolean")
            });

            PropertyDataTypeResolver resolver = new PropertyDataTypeResolver(contentType);

            // Act
            bool successfullyResolvedProperty =
                resolver.TryResolve(propertyName, out PropertyDataType resolvedPropertyDataType);

            // Assert
            successfullyResolvedProperty.Should().BeTrue();
            resolvedPropertyDataType.ShouldBeEquivalentTo(propertyDataType);
        }

        [Fact]
        public void Test_when_property_type_cannot_be_resolved_should_return_false()
        {
            // Arrange
            ContentType contentType = new ContentType();
            contentType.PropertyDefinitions.Add(new PropertyDefinition
            {
                Name = "TestProperty",
                Type = new PropertyDefinitionType(1, PropertyDataType.Boolean, "Boolean")
            });

            PropertyDataTypeResolver resolver = new PropertyDataTypeResolver(contentType);

            // Act
            bool successfullyResolvedProperty =
                resolver.TryResolve("ThisPropertyDoesntExist", out PropertyDataType resolvedPropertyDataType);

            // Assert
            successfullyResolvedProperty.Should().BeFalse();
            resolvedPropertyDataType.ShouldBeEquivalentTo(PropertyDataType.String);
        }
    }
}
