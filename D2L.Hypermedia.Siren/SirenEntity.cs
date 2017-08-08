﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace D2L.Hypermedia.Siren {

	public class SirenEntity : ISirenEntity {

		private readonly string m_type;
		private readonly Uri m_href;
		private readonly string[] m_rel;
		private readonly string m_title;
		private readonly IEnumerable<ISirenAction> m_actions;
		private readonly IEnumerable<ISirenLink> m_links;
		private readonly IEnumerable<ISirenEntity> m_entities;
		private readonly dynamic m_properties;
		private readonly string[] m_class;

		public SirenEntity(
			string[] rel = null,
			string[] @class = null,
			dynamic properties = null,
			IEnumerable<ISirenEntity> entities = null,
			IEnumerable<ISirenLink> links = null,
			IEnumerable<ISirenAction> actions = null,
			string title = null,
			Uri href = null,
			string type = null
		) {
			m_rel = rel ?? new string[0];
			m_class = @class ?? new string[0];
			m_properties = properties;
			m_entities = entities ?? new List<ISirenEntity>();
			m_links = links ?? new List<ISirenLink>();
			m_actions = actions ?? new List<ISirenAction>();
			m_title = title;
			m_href = href;
			m_type = type;
		}

		[JsonProperty( "class", NullValueHandling = NullValueHandling.Ignore )]
		public string[] Class {
			get { return m_class; }
		}

		[JsonProperty( "properties", NullValueHandling = NullValueHandling.Ignore )]
		public dynamic Properties {
			get { return m_properties; }
		}

		[JsonProperty( "entities", NullValueHandling = NullValueHandling.Ignore )]
		[JsonConverter( typeof(HypermediaEntityConverter) )]
		public IEnumerable<ISirenEntity> Entities {
			get { return m_entities; }
		}

		[JsonProperty( "links", NullValueHandling = NullValueHandling.Ignore )]
		[JsonConverter( typeof(HypermediaLinkConverter) )]
		public IEnumerable<ISirenLink> Links {
			get { return m_links; }
		}

		[JsonProperty( "actions", NullValueHandling = NullValueHandling.Ignore )]
		[JsonConverter( typeof(HypermediaActionConverter) )]
		public IEnumerable<ISirenAction> Actions {
			get { return m_actions; }
		}

		[JsonProperty( "title", NullValueHandling = NullValueHandling.Ignore )]
		public string Title {
			get { return m_title; }
		}

		[JsonProperty( "rel", NullValueHandling = NullValueHandling.Ignore )]
		public string[] Rel {
			get { return m_rel; }
		}

		[JsonProperty( "href", NullValueHandling = NullValueHandling.Ignore )]
		public Uri Href {
			get { return m_href; }
		}

		[JsonProperty( "type", NullValueHandling = NullValueHandling.Ignore )]
		public string Type {
			get { return m_type; }
		}

		public bool ShouldSerializeRel() {
			return Rel.Length > 0;
		}

		public bool ShouldSerializeClass() {
			return Class.Length > 0;
		}

		public bool ShouldSerializeEntities() {
			return Entities.Any();
		}

		public bool ShouldSerializeLinks() {
			return Links.Any();
		}

		public bool ShouldSerializeActions() {
			return Actions.Any();
		}

		bool IEquatable<ISirenEntity>.Equals( ISirenEntity other ) {
			if( other == null ) {
				return false;
			}

			bool rel = m_rel.OrderBy( x => x ).SequenceEqual( other.Rel.OrderBy( x => x ) );
			bool @class = m_class.OrderBy( x => x ).SequenceEqual( other.Class.OrderBy( x => x ) );
			bool properties = m_properties == other.Properties
				|| ( m_properties != null
					&& other.Properties != null
					&& m_properties.ToString().Equals( other.Properties.ToString() ) );
			bool entities = m_entities.OrderBy( x => x ).SequenceEqual( other.Entities.OrderBy( x => x ) );
			bool links = m_links.OrderBy( x => x ).SequenceEqual( other.Links.OrderBy( x => x ) );
			bool actions = m_actions.OrderBy( x => x ).SequenceEqual( other.Actions.OrderBy( x => x ) );
			bool title = m_title == other.Title;
			bool href = m_href == other.Href;
			bool type = m_type == other.Type;

			return rel && @class && properties && entities && links && actions && title && href && type;
		}

		int IComparable<ISirenEntity>.CompareTo( ISirenEntity other ) {
			if( other == null ) {
				return 1;
			}

			return string.CompareOrdinal( m_title, other.Title );
		}

		int IComparable.CompareTo( object obj ) {
			ISirenEntity @this = this;
			return @this.CompareTo( (ISirenEntity)obj );
		}

		bool IContains<ISirenEntity>.Contains( ISirenEntity other ) {
			bool isSubset = true;
			if( other.Rel != null && other.Rel.Any() ) {
				isSubset = isSubset && other.Rel.All( s => m_rel.Contains( s ) );
			}
			if( other.Class != null && other.Class.Any() ) {
				isSubset = isSubset && other.Class.All( s => m_class.Contains( s ) );
			}
			if( other.Properties != null ) {
				isSubset = isSubset &&
					( m_properties == other.Properties
					|| ( m_properties != null
						&& other.Properties != null
						&& m_properties.ToString().Equals( other.Properties.ToString() ) ) );
			}
			if( other.Entities != null && other.Entities.Any() ) {
				isSubset = isSubset && other.Entities.All( x => m_entities.Any( y => y.Contains( x ) ) );
			}
			if( other.Links != null && other.Links.Any() ) {
				isSubset = isSubset && other.Links.All( x => m_links.Any( y => y.Contains( x ) ) );
			}
			if( other.Actions != null && other.Actions.Any() ) {
				isSubset = isSubset && other.Actions.All( x => m_actions.Any( y => y.Contains( x ) ) );
			}
			if( other.Href != null ) {
				isSubset = isSubset && m_href == other.Href;
			}
			if( other.Title != null ) {
				isSubset = isSubset && m_title == other.Title;
			}
			if( other.Type != null ) {
				isSubset = isSubset && m_type == other.Type;
			}

			return isSubset;
		}

		public override bool Equals( object obj ) {
			ISirenEntity entity = obj as ISirenEntity;
			ISirenEntity @this = this;
			return entity != null && @this.Equals( entity );
		}

		public override int GetHashCode() {
			return string.Join( ",", m_rel ).GetHashCode()
				^ string.Join( ",", m_class ).GetHashCode()
				^ m_properties?.GetHashCode() ?? 0
				^ string.Join( ",", m_entities ).GetHashCode()
				^ string.Join( ",", m_links ).GetHashCode()
				^ string.Join( ",", m_actions ).GetHashCode()
				^ m_title?.GetHashCode() ?? 0
				^ m_href?.GetHashCode() ?? 0
				^ m_type?.GetHashCode() ?? 0;

		}

	}

	public class HypermediaEntityConverter : JsonConverter {


		public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer ) {
			serializer.Serialize( writer, value );
		}

		public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer ) {
			return serializer.Deserialize<SirenEntity[]>( reader );
		}

		public override bool CanConvert( Type objectType ) {
			return objectType == typeof( SirenEntity );
		}

	}

}